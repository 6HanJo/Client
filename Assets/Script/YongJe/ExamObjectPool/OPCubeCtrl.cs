using UnityEngine;
using System.Collections;
using PathologicalGames;

public class OPCubeCtrl : MonoBehaviour
{

    void OnSpawned()
    {
        Debug.Log("Spawnd " + name);
        StartCoroutine(DespawnedObject(5f));
    }

    IEnumerator DespawnedObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        PoolManager.Pools["Test"].Despawn(this.transform);
    }

    void OnDespawned()
    {
        Debug.Log("Despawned " + name);
    }
}
