using UnityEngine;
using System.Collections;
//오브젝트풀 사용을 위한 모듈 추가
using PathologicalGames;

public class OPSpawnManager : MonoBehaviour
{
    public Transform spawnPrefab;

    //스폰딜레이 sec
    public float delay = 1f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(SpawnObject());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(delay);
        print(PoolManager.Pools["Test"].name);
        Transform spawnInstance;
        SpawnPool spawnPool = PoolManager.Pools["Test"];
        spawnInstance = spawnPool.Spawn(spawnPrefab);
        spawnInstance.position = new Vector3(10f, 0f, 0f);
        StartCoroutine(SpawnObject());
    }
}
