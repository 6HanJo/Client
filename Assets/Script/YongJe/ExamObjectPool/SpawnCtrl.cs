using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnCtrl : MonoBehaviour
{
    static SpawnPool spawnPool = null;
    public float delay = 2f;

    Transform tr;

    void Awake() {
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        if (spawnPool == null)
        {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

    public void SetActives() {
        spawnPool.Despawn(tr);
    }

    void OnSpawned()
    {
        StartCoroutine(DespawnedObject(delay));
    }

    IEnumerator DespawnedObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnPool.Despawn(tr);
    }

    void OnDespawned()
    {
        //Debug.Log("Despawned " + name);
    }
}
