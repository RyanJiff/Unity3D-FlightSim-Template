using UnityEngine;
using System;
using System.Collections.Generic;

public class Airplane : MonoBehaviour, IControllable
{
	/*
	 * Main airplane driver class
	 */

	[Header("Mouse Yoke")]
	public bool mouseYoke = true;
	public float deadZone = 0.05f;
	[Space]

	[Header("Control Surfaces")]
	public ControlSurface elevator;
	public ControlSurface aileronLeft;
	public ControlSurface aileronRight;
	public ControlSurface rudder;
	[Space]

	[Header("Engines")]
	public Engine engine;
	[Space]

	[Header("Brakes")]
	public WheelCollider[] brakeWheels;
	public int brakeTorque;
	[Space]

	[Header("Steering")]
	public WheelCollider[] steeringWheels;
	public float maxSteerAngle = 20;
	public bool invertedSteering = false;
	[Space]

	[Header("Center of mass ")]
	public Transform centerOfMassTransform;

	public Rigidbody Rigidbody { get; internal set; }
	private LandingGear landingGear = null;
	private FuelTank[] fuelTanks = null;

	private bool yawDefined = false;

	public float pitch;
	public float roll;

	// Inputs that are changed through external MonoBehaviours
	private float _inputRoll = 0;
	private float _inputPitch = 0;
	private float _inputYaw = 0;
	private bool _inputBrake = false;
	private float _inputThrottle = 0;
	public float elevatorTrim = 0.10f;


	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		landingGear = GetComponent<LandingGear>();
		fuelTanks = GetComponents<FuelTank>();

		if (centerOfMassTransform)
		{
			Rigidbody.centerOfMass = centerOfMassTransform.transform.localPosition;
		}
		else
		{
			Debug.LogWarning(name + ": Airplane missing center of mass transform!");
		}
	}

	private void Start()
	{
		if (elevator == null)
			Debug.LogWarning(name + ": Airplane missing elevator!");
		if (aileronLeft == null)
			Debug.LogWarning(name + ": Airplane missing left aileron!");
		if (aileronRight == null)
			Debug.LogWarning(name + ": Airplane missing right aileron!");
		if (rudder == null)
			Debug.LogWarning(name + ": Airplane missing rudder!");
		if (engine == null)
			Debug.LogWarning(name + ": Airplane missing engine!");

		try
		{
			Input.GetAxis("Yaw");
			yawDefined = true;
		}
		catch (ArgumentException e)
		{
			Debug.LogWarning(e);
			Debug.LogWarning(name + ": \"Yaw\" axis not defined in Input Manager. Rudder will not work correctly!");
		}
	}

	void Update()
	{
		//control surfaces input
		if (elevator != null)
		{
			elevator.targetDeflection = _inputPitch;
		}
		if (aileronLeft != null)
		{
			aileronLeft.targetDeflection = -_inputRoll;
		}
		if (aileronRight != null)
		{
			aileronRight.targetDeflection = _inputRoll;
		}
		if (rudder != null && yawDefined)
		{
			rudder.targetDeflection = _inputYaw;

			foreach (WheelCollider c in steeringWheels)
			{
				if (!invertedSteering)
				{
					c.steerAngle = _inputYaw * maxSteerAngle;
				}
				else
				{
					c.steerAngle = _inputYaw * -maxSteerAngle;
				}
			}
		}

		//clamp elevator trim
		elevatorTrim = Mathf.Clamp(elevatorTrim, -0.8f, 0.8f);

		//brakes
		if (_inputBrake)
		{
			foreach (WheelCollider w in brakeWheels)
			{
				w.brakeTorque = brakeTorque;
			}
		}
		else
		{
			foreach (WheelCollider w in brakeWheels)
			{
				w.brakeTorque = 0;
				//we have to give the motor some torque when not braking so it does not lock up and go into sleep state.
				w.motorTorque = 0.00000001f;
			}
		}

		//engine controls
		if (engine != null)
		{
			engine.throttleInput = _inputThrottle;
		}

		//pitch
		Vector3 pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.forward);
		pitch = SignedAngle(transform.forward, pos, transform.right);

		// roll
		pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.right);
		roll = SignedAngle(transform.right, pos, transform.forward);
	}

    #region calculations
    private float CalculatePitchG()
	{
		// Angular velocity is in radians per second.
		Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
		Vector3 localAngularVel = transform.InverseTransformDirection(Rigidbody.angularVelocity);

		// Local pitch velocity (X) is positive when pitching down.

		// Radius of turn = velocity / angular velocity
		float radius = (Mathf.Approximately(localAngularVel.x, 0.0f)) ? float.MaxValue : localVelocity.z / localAngularVel.x;

		// The radius of the turn will be negative when in a pitching down turn.

		// Force is mass * radius * angular velocity^2
		float verticalForce = (Mathf.Approximately(radius, 0.0f)) ? 0.0f : (localVelocity.z * localVelocity.z) / radius;

		// Express in G (Always relative to Earth G)
		float verticalG = verticalForce / -9.81f;

		// Add the planet's gravity in. When the up is facing directly up, then the full
		// force of gravity will be felt in the vertical.
		verticalG += transform.up.y * (Physics.gravity.y / -9.81f);

		return verticalG;
	}

	Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
	{
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal.normalized, (point - planePoint));
		return point + planeNormal * distance;
	}

	float SignedAngle(Vector3 v1, Vector3 v2, Vector3 normal)
	{
		Vector3 perp = Vector3.Cross(normal, v1);
		float angle = Vector3.Angle(v1, v2);
		angle *= Mathf.Sign(Vector3.Dot(perp, v2));
		return angle;
	}
    

    public float AirSpeed()
	{
		const float msToKnots = 1.94384f;
		return Rigidbody.velocity.magnitude * msToKnots;
	}
	public float VerticalSpeed()
	{
		return Rigidbody.velocity.y * 3.28084f * 60;
	}
	#endregion

	/// <summary>
	/// Send inputs to airplane
	/// </summary>
	public void SendInput(float y, float x, float z, bool brake, float throttle)
	{
		_inputPitch = y;
		_inputRoll = x;
		_inputYaw = z;
		_inputBrake = brake;
		_inputThrottle = throttle;
	}

	/// <summary>
	/// change trim by amount
	/// </summary>
	public void TrimChange(float amount)
	{
		elevatorTrim += amount;
	}

	public float GetTrim()
    {
		return elevatorTrim;
    }
	/// <summary>
	/// toggle gear if we have one
	/// </summary>
	public void ToggleGear()
    {
		if (landingGear)
        {
			landingGear.ToggleGear();
        }
        else
        {
			Debug.Log("No retractable gear!");
        }
    }
	public void ToggleEngine()
    {
		if (engine)
		{
			engine.ToggleIgnition();
		}
    }
	/// <summary>
	/// return amount of fuel in all tanks
	/// </summary>
	public float GetFuelAmount()
	{
		float fuelAmount = 0f;
		foreach (FuelTank f in fuelTanks)
		{
			fuelAmount += f.GetCurrentFuelAmount();
		}
		return fuelAmount;
	}
}
