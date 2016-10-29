using UnityEngine;
using System.Collections;
using PathologicalGames;

public class BentSpiralShooter : MonoBehaviour {
	public float ShotAngle;
	public float ShotAngleRate;
	public int ShotCount;
	public float ShotSpeed;
	public int Interval;
	private int Timer;
	public float BulletAngleRate;
	public float BulletSpeedRate;
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

    void Update () {
		if (Timer == 0 && canShoot)
		{
			for (int i = 0; i < ShotCount; i++)
			{
				tmp = spawnPool.Spawn(bullet);
				tBullet = tmp.GetComponent<Bullet> ();
				tmp.transform.position = transform.position;
				tBullet.speed = ShotSpeed;
				tBullet.angle = ShotAngle + (float)i / ShotCount;
				tBullet.angleRate = BulletAngleRate;
				tBullet.speedRate = BulletSpeedRate;
			}
			ShotAngle += ShotAngleRate;
			ShotAngle -= Mathf.Floor(ShotAngle);
		}
		Timer = (Timer + 1) % Interval;
	}
}
