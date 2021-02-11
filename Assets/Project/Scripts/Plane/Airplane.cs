//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ControlSurfaces
{
	public ControlSurface elevator;
	public ControlSurface aileronLeft;
	public ControlSurface aileronRight;
	public ControlSurface rudder;
}



public class Airplane : MonoBehaviour
{

	[Header("Mouse Yoke")]
	public bool mouseYoke = true;
	public float deadZone = 0.1f;
	[Space]

	[Header("Control Surfaces")]
	public ControlSurfaces controlSurfaces;
	[Space]
	
	[Header("Engines")]
	public Engine engine;
	[Space]

	[Header("Fueselage Stats")]
	public float baseDrag = 0.01f;
	public float extendedGearDrag = 0.01f;

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
	private LandingGear landingGear;
	
	private float throttle = 0.0f;
	private bool yawDefined = false;

	private float pitch;
	private float roll;


	[Header("Debug")]
	public bool grounded = true;
	[SerializeField]
	public float elevatorTrim = 0.2f;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();
		landingGear = GetComponent<LandingGear>();

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
		if (controlSurfaces.elevator == null)
			Debug.LogWarning(name + ": Airplane missing elevator!");
		if (controlSurfaces.aileronLeft == null)
			Debug.LogWarning(name + ": Airplane missing left aileron!");
		if (controlSurfaces.aileronRight == null)
			Debug.LogWarning(name + ": Airplane missing right aileron!");
		if (controlSurfaces.rudder == null)
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

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Y))
		{
			mouseYoke = !mouseYoke;
		}

		if (controlSurfaces.elevator != null)
		{
			controlSurfaces.elevator.targetDeflection = Mathf.Clamp(-Input.GetAxis("Vertical") - MouseControlY() + elevatorTrim , -1, 1);
		}
		if (controlSurfaces.aileronLeft != null)
		{
			controlSurfaces.aileronLeft.targetDeflection = Mathf.Clamp(-Input.GetAxis("Horizontal") - MouseControlX(), -1, 1);
		}
		if (controlSurfaces.aileronRight != null)
		{
			controlSurfaces.aileronRight.targetDeflection = Mathf.Clamp(Input.GetAxis("Horizontal") + MouseControlX(), -1, 1);
		}
		if (controlSurfaces.rudder != null && yawDefined)
		{
			controlSurfaces.rudder.targetDeflection = Input.GetAxis("Yaw");

			foreach (WheelCollider c in steeringWheels)
			{
				if (!invertedSteering)
				{
					c.steerAngle = Input.GetAxis("Yaw") * maxSteerAngle;
				}
				else
				{
					c.steerAngle = Input.GetAxis("Yaw") * -maxSteerAngle;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.Equals))
			elevatorTrim += 0.02f;

		if (Input.GetKeyDown(KeyCode.Minus))
			elevatorTrim -= 0.02f;

		elevatorTrim = Mathf.Clamp(elevatorTrim, -0.8f, 0.8f);

		if (Input.GetKey(KeyCode.B))
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
				w.motorTorque = 0.00000001f;
			}
		}

		if (engine != null)
		{
			if (Input.GetKey(KeyCode.Alpha2))
			{
				throttle += 0.5f * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.Alpha1))
			{
				throttle -= 0.8f * Time.deltaTime;
			}

			throttle = Mathf.Clamp01(throttle);
			engine.throttleInput = throttle;

			if (Input.GetKeyDown(KeyCode.I))
			{
				engine.StartStopEngine();
			}
		}

		//pitch
		Vector3 pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.forward);
		pitch = SignedAngle(transform.forward, pos, transform.right);

		// roll
		pos = ProjectPointOnPlane(Vector3.up, Vector3.zero, transform.right);
		roll = SignedAngle(transform.right, pos, transform.forward);

		


		CalculateDrag();
	}

	void CalculateDrag()
	{
		if (landingGear)
		{
			if (landingGear.extended)
			{
				Rigidbody.drag = baseDrag + extendedGearDrag;
			}
			else
			{
				Rigidbody.drag = baseDrag;
			}
		}
		else
		{
			Rigidbody.drag = baseDrag;
		}
	}



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

	private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.fontStyle = FontStyle.Bold;

		
		GUI.Label(new Rect(10, 40, 300, 20), string.Format("Speed: {0:0} kn", AirSpeed()), style);
		GUI.Label(new Rect(10, 60, 300, 20), string.Format("Throttle: {0:0}%", throttle * 100.0f), style);
		GUI.Label(new Rect(10, 80, 300, 20), string.Format("RPM: {0:0}", engine.GetRPM()), style);
		GUI.Label(new Rect(10, 100, 300, 20), string.Format("VSI: {0:0} Feet Per Minute", VerticalSpeed()), style);

		GUI.Label(new Rect(10, 140, 300, 20), "Throttle Control (1,2) Change View (C) Engine Toggle (I) Toggle Mouse Yoke (Y)", style);
		GUI.Label(new Rect(10, 160, 400, 20), "Controls: Elevator(W,S) Aileron(A,D) Rudder(Q,E) Trim(-,+) Brakes(B)", style);
		GUI.Label(new Rect(10, 180, 300, 20), "(BACKSPACE) to retry, (ESC) to exit", style);
		GUI.Label(new Rect(10, 200, 300, 20), string.Format("Elevator Trim: {0:0.00} ", elevatorTrim), style);
		

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

	void MouseControlDebug()
	{
		Debug.Log("Roll: " + ((Input.mousePosition.x*2 / Screen.width) - 1));
		Debug.Log("Pitch: " + ((Input.mousePosition.y*2 / Screen.height) - 1));
	}

	float MouseControlX()
	{
		float xPoint = ((Input.mousePosition.x * 2 / Screen.width) - 1);
		if (Mathf.Abs(xPoint) > deadZone && mouseYoke)
		{
			return xPoint;
		}
		else return 0;
		
	}
	float MouseControlY()
	{
		float yPoint = ((Input.mousePosition.y * 2 / Screen.height) - 1);
		if (Mathf.Abs(yPoint) > deadZone && mouseYoke)
		{
			return yPoint;
		}
		else return 0;
	}

}
