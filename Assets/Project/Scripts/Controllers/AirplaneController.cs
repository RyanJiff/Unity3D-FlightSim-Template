using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    /*
     * Airplane Controller manages inputs of player and sends them to the airplane directly
     */

    [SerializeField]
	Airplane airplane = null;

    private float _inputRoll = 0;
    private float _inputPitch = 0;
    private float _inputYaw = 0;
    private bool _inputBrake = false;
	private float _inputThrottle = 0;

    public bool mouseYoke = true;
	public float deadZone = 0.05f;

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Y))
			mouseYoke = !mouseYoke;

		if (airplane)
		{
			//engine toggle
			if (Input.GetKeyDown(KeyCode.I))
				airplane.ToggleEngine();

			//gear input
			if (Input.GetKeyDown(KeyCode.G))
				airplane.ToggleGear();

			//trim input
			if (Input.GetKeyDown(KeyCode.Equals))
				airplane.TrimChange(0.02f);

			if (Input.GetKeyDown(KeyCode.Minus))
				airplane.TrimChange(-0.02f);

			
			//control surfaces and brakes
			_inputRoll = Mathf.Clamp(Input.GetAxis("Horizontal") + MouseControlX(), -1, 1);
			_inputPitch = Mathf.Clamp(-Input.GetAxis("Vertical") - MouseControlY() + airplane.GetTrim(), -1, 1);
			_inputYaw = Input.GetAxis("Yaw");
			_inputBrake = Input.GetKey(KeyCode.B);

			//throttle controls
			if (Input.GetKey(KeyCode.Alpha2))
			{
				_inputThrottle += 0.5f * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.Alpha1))
			{
				_inputThrottle -= 0.8f * Time.deltaTime;
			}
			_inputThrottle = Mathf.Clamp01(_inputThrottle);

			//send inputs
			airplane.SendInput(_inputPitch, _inputRoll, _inputYaw, _inputBrake,_inputThrottle);
		}
	}

	/// <summary>
	/// get X axis for mouse yoke controls
	/// </summary>
	float MouseControlX()
	{
		float xPoint = ((Input.mousePosition.x * 2 / Screen.width) - 1);
		if (Mathf.Abs(xPoint) > deadZone && mouseYoke)
		{
			return xPoint;
		}
		else return 0;

	}
	/// <summary>
	/// get Y axis for mouse yoke controls
	/// </summary>
	float MouseControlY()
	{
		float yPoint = ((Input.mousePosition.y * 2 / Screen.height) - 1);
		if (Mathf.Abs(yPoint) > deadZone && mouseYoke)
		{
			return yPoint;
		}
		else return 0;
	}

	public void GiveControl(Airplane a)
    {
		airplane = a;
    }
}
