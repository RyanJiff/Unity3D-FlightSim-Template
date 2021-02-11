using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public string scene_MainMenu;


    public string scene_OpenWorld;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Application.targetFrameRate = 60;
    }
    public void LoadOpenWorld()
    {
        SceneManager.LoadScene(scene_OpenWorld);
    }
    
    public void Button_MainMenu()
    {
        SceneManager.LoadScene(scene_MainMenu);
    }
    public void Button_Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Button_Exit()
    {
        Application.Quit();
    }

}
