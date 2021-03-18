using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    /*
     * Manages world world spawn locations.
     */
    [System.Serializable]
    public class SpawnLocation
    {
        public Vector3 position = Vector3.zero;
        public string _name = "_spawn location";
    }

    [SerializeField]
    SpawnLocation[] spawnLocations = null;


    public SpawnLocation GetSpawnLocation(int i)
    {
        if (spawnLocations.Length > i)
        {
            return spawnLocations[i];
        }

        return null;
    }
}
