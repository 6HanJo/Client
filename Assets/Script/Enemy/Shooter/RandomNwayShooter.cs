using UnityEngine;
using System.Collections;
using PathologicalGames;

public class RandomNwayShooter : MonoBehaviour
{
    public float ShotAngleRange;
    public float ShotSpeed;
    public int ShotCount;
    public int Interval;
    private int Timer;
    public GameObject bullet;
    public bool canShoot = true;

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
                tBullet.speed = ShotSpeed;
				tBullet.speedRate = 0;
				tBullet.angleRate = 0;
				tBullet.angle = EnemyLib.instance.GetPlayerAngle(transform.position) + ShotAngleRange * (Random.Range(0.0f, 1.0f) - 0.5f);
            }
        }
        if (canShoot)
            Timer = (Timer + 1) % Interval;
        if (!canShoot)
            Timer = 0;
    }
}
