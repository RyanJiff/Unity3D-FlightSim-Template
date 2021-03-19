using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AirplaneController))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(PlayerGUI))]
public class PlayerController : MonoBehaviour
{
    /*
     * Handles handing player control over a vehicle.
     */
    AirplaneController myController = null;
    CameraController myCameraController = null;
    PlayerGUI myPlayerGUI = null;

    void Awake()
    {
        myController = GetComponent<AirplaneController>();
        myCameraController = GetComponent<CameraController>();
        myPlayerGUI = GetComponent<PlayerGUI>();
    }



    /// <summary>
    /// take control of airplane gameobject
    /// </summary>
    public void TakeControl(GameObject airplane)
    {
        if(airplane.GetComponent<Airplane>() != null)
        {
            myController.GiveControl(airplane.GetComponent<Airplane>());
            myCameraController.target = airplane.transform;
            myPlayerGUI.airplane = airplane.GetComponent<Airplane>();
        }
        else
        {
            Debug.LogWarning("Tried to take control of non controllable object!");
        }
    }
}
