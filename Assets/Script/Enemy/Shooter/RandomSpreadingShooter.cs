using UnityEngine;
using System.Collections;
using PathologicalGames;

public class RandomSpreadingShooter : MonoBehaviour
{
    public float ShotAngleRange;
    public float ShotSpeed;
    public float ShotSpeedRange;
    public int ShotCount;
    public int Interval;
    private int Timer;
    public bool canShoot = true;
    public GameObject bullet;

    static SpawnPool spawnPool = null;
    Transform tmp, tr;
    Bullet tBullet;

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

    void Update()
    {
        if (Timer == 0 && canShoot)
        {
            for (int i = 0; i < ShotCount; i++)
            {
                tmp = spawnPool.Spawn(bullet);
                tBullet = tmp.GetComponent<Bullet>();
                tmp.transform.position = tr.position;
                tBullet.speed = ShotSpeed + ShotSpeedRange * (Random.Range(0.0f, 1.0f) - 0.5f);
                tBullet.angle = EnemyLib.instance.GetPlayerAngle(transform.position) + ShotAngleRange * (Random.Range(0.0f, 1.0f) - 0.5f);
				tBullet.speedRate = 0;
				tBullet.angleRate = 0;
            }
        }
        if (canShoot)
            Timer = (Timer + 1) % Interval;
        if (!canShoot)
            Timer = 0;
    }
}
