using UnityEngine;

//This is a basic interface for controllable vehicles
public interface IControllable
{
    void SendInput(float y, float x, float z, bool brake, float throttle);
    void ToggleEngine();

}
