using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initialize : MonoBehaviour
{
    /*
     * when the application first starts this script runs
     */

    void Start()
    {
        SceneManager.LoadScene("mainmenu");
    }
}
