using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*
     *Responsible for managing current game session entities.
     */

    //player prefab to spawn
    public GameObject playerPrefab = null;

    public string mainMenuScene;

    [System.Serializable]
    public class InitialSessionData
    {
        //scenary and terrain to load
        public GameObject worldPrefab = null;
        //player airplane to load
        public GameObject playerAirplanePrefab = null;

        public int spawnLocationIndex = 0;
    }
    [System.Serializable]
    public class SessionData
    {
        //scenary and terrain to load
        public GameObject world = null;
        //player airplane to load
        public GameObject playerAirplane = null;
        //player prefab to spawn
        public GameObject player = null;
    }

    //session data to use when loading a new scene.
    [SerializeField]
    InitialSessionData initialSessionData = null;
    [SerializeField]
    SessionData currentSession = null;

    void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.initialSessionData = null;
            this.currentSession = null;
            SceneManager.LoadScene(mainMenuScene,0);
        }
    }

    /// <summary>
    /// called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Scene " + arg0.name + " Loaded.");
        InitSession();
    }

    /// <summary>
    /// spawn player and entities
    /// </summary>
    void InitSession()
    {
        //clea current session
        this.currentSession = null;
        
        if (initialSessionData == null)
        {
            //if there is no initial session data it means we are not going to a session but probably to a menu.
            return;
        }
        
        currentSession = new SessionData();

        //init world and get spawn location
        currentSession.world = Instantiate(initialSessionData.worldPrefab);
        Vector3 spawnLoc = currentSession.world.GetComponent<WorldManager>().GetSpawnLocation(initialSessionData.spawnLocationIndex).position;

        //spawn player and airplane.
        currentSession.player = Instantiate(playerPrefab,spawnLoc,Quaternion.identity);
        currentSession.playerAirplane = Instantiate(initialSessionData.playerAirplanePrefab,spawnLoc,Quaternion.identity);

        //hand player control of airplane
        currentSession.player.GetComponent<PlayerController>().TakeControl(currentSession.playerAirplane);

        this.initialSessionData = null;
    }

    public void SetInitialSessionData(InitialSessionData isd)
    {
        this.initialSessionData = isd;
        Debug.Log("Initial Session data set.");
    }
}
