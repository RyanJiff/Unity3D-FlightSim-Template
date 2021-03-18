using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AirplaneController))]
[RequireComponent(typeof(SimpleCamera))]
[RequireComponent(typeof(PlayerGUI))]
public class PlayerController : MonoBehaviour
{
    /*
     * Handles handing player control over a vehicle.
     */
    AirplaneController myController = null;
    SimpleCamera myCameraController = null;
    PlayerGUI myPlayerGUI = null;

    private void Awake()
    {
        myController = GetComponent<AirplaneController>();
        myCameraController = GetComponent<SimpleCamera>();
        myPlayerGUI = GetComponent<PlayerGUI>();
    }

    public void TakeControl(GameObject airplane)
    {
        if(airplane.GetComponent<Airplane>() != null)
        {
            myController.GiveControl(airplane.GetComponent<Airplane>());
            myCameraController.target = airplane.transform;
            myPlayerGUI.airplane = airplane.GetComponent<Airplane>();
        }
    }
}
