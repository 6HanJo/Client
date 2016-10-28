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
    
    // Use this for initialization
    void Start () {
        spawnPool = PoolManager.Pools["Test"];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
