using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
	public Airplane airplane = null;

	bool showControls = false;

	private void OnGUI()
	{
		GUIStyle style = new GUIStyle();
		style.fontSize = 16;
		style.fontStyle = FontStyle.Bold;

		if (airplane)
		{

			GUI.Label(new Rect(10, 40, 300, 20), string.Format("Speed: {0:0} kn", airplane.AirSpeed()), style);
			GUI.Label(new Rect(10, 60, 300, 20), string.Format("Throttle: {0:0}%", airplane.engine.throttleInput * 100.0f), style);
			GUI.Label(new Rect(10, 80, 300, 20), string.Format("RPM: {0:0}", airplane.engine.GetRPM()), style);
			GUI.Label(new Rect(10, 100, 300, 20), string.Format("Altitude: {0:0} Feet", airplane.transform.position.y * 3.28084f), style);
			GUI.Label(new Rect(10, 120, 300, 20), string.Format("VSI: {0:0} Feet Per Minute", airplane.VerticalSpeed()), style);
			GUI.Label(new Rect(10, 140, 300, 20), string.Format("Elevator Trim: {0:0.00} ", airplane.elevatorTrim), style);
			GUI.Label(new Rect(10, 160, 300, 20), string.Format("Pitch: {0:0}", airplane.pitch), style);
			GUI.Label(new Rect(10, 180, 300, 20), string.Format("Fuel: {0:0.0} Litres", airplane.GetFuelAmount()), style);

			if (showControls)
			{
				GUI.Label(new Rect(10, 220, 300, 20), "Throttle Control (1,2) Look Back (C) Engine Toggle (I) Toggle Mouse Yoke (Y)", style);
				GUI.Label(new Rect(10, 240, 400, 20), "Controls: Elevator(W,S) Aileron(A,D) Rudder(Q,E) Trim(-,+) Brakes(B)", style);
				GUI.Label(new Rect(10, 260, 300, 20), " ", style);
			}

			if(GUI.Button(new Rect(10, 200, 200, 20), "Show Controls"))
			{
				showControls = !showControls;
            }

		}
	}
}
