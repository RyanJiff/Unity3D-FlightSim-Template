using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*
     * Simple main menu
     */

    GameManager gameManager;

    [SerializeField]
    GameManager.InitialSessionData initialSessionData = null;

    
    public string freeFlightScene;

    void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(Screen.width/2 - 125, Screen.height / 2 - 150,250,100),"Start Free Flight"))
        {
            gameManager.SetInitialSessionData(initialSessionData);
            SceneManager.LoadScene(freeFlightScene,0);
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 125, Screen.height / 2 + 150, 250, 100), "Quit"))
        {
            Application.Quit();
        }
    }
}
