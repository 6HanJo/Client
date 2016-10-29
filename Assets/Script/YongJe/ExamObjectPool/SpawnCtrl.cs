using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnCtrl : MonoBehaviour
{
    static SpawnPool spawnPool = null;

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

    void OnSpawned() {

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

}
