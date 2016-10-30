using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnCtrl : MonoBehaviour
{
    static SpawnCtrl instance;
    public static SpawnCtrl Instance
    {
         get { return instance; }
    }
    
    public static SpawnPool spawnPool = null;

    Transform tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        if (spawnPool == null)
        {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

    public void SetTimeDespawn(float time) {
        StartCoroutine("Delay", time);
    }

    void OnSpawned()
    {
        tr.localScale = new Vector3(1, 1, 1);
    }

    public void SetActives()
    {
        spawnPool.Despawn(tr);
        tr.localScale = new Vector3(1, 1, 1);
    }

    void OnBecameInvisible()
    {
        if (gameObject.activeSelf)
        {
            spawnPool.Despawn(tr);
            tr.localScale = new Vector3(1, 1, 1);
        }
    }

    IEnumerator Delay(float time) {
        yield return new WaitForSeconds(time);
        SetActives();
    }

}
