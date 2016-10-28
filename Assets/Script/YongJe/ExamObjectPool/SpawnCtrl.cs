﻿using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnCtrl : MonoBehaviour
{
    static SpawnPool spawnPool = null;

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

    void OnBecameInvisible()
    {
        spawnPool.Despawn(tr);
    }
}
