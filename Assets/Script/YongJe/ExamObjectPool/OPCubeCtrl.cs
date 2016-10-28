using UnityEngine;
using System.Collections;
using PathologicalGames;

public class OPCubeCtrl : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
