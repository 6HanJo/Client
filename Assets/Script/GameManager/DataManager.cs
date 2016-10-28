using UnityEngine;
using System.Collections;
using PathologicalGames;

public class DataManager : MonoBehaviour {

    static DataManager instance;
    public static DataManager Instance
    {
        get { return instance; }
    }
    static SpawnPool spawnPool;
    
    void Awake()
    {
        instance = this;
    }
    
    void Start () {
        spawnPool = PoolManager.Pools["Test"];
	}
	
}
