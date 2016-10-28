using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnCtrl : MonoBehaviour
{
    static SpawnPool spawnPool = null;
    public float delay;

    void Start()
    {
        if (spawnPool == null)
        {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

    void OnSpawned()
    {
        StartCoroutine(DespawnedObject(2f));
    }

    IEnumerator DespawnedObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnPool.Despawn(this.transform);
    }

    void OnDespawned()
    {
        //Debug.Log("Despawned " + name);
    }
}
