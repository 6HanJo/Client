using UnityEngine;
using System.Collections;
using PathologicalGames;

public class IntervalMultipleSpiralShooter : MonoBehaviour {
	public float shotAngle;
	public float shotAngleRate;
	public float shotSpeed;
	public float shotDelay;
	public int shotCount;
	public bool canShoot = true;
	public GameObject bullet;
	public float bulletMoney;
	public float bulletHp;

    static SpawnPool spawnPool = null;
    Transform tmp, tr;
    Bullet tBullet;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Start()
	{
		if (spawnPool == null) {
			spawnPool = PoolManager.Pools ["Test"];
		}
		StartCoroutine ("SpawnObject");
	}

    IEnumerator SpawnObject()
	{
		while (true) {
			if (canShoot) {
				if (shotAngle >= 1)
					shotAngle = 0;
				shotAngle += shotCount * shotAngleRate;
				for (int i = 0; i < shotCount; i++) {
					tmp = spawnPool.Spawn (bullet, Vector3.zero, Quaternion.identity);
					tBullet = tmp.GetComponent<Bullet> ();
					tmp.transform.position = tr.position;
					tBullet.speed = shotSpeed;
					tBullet.angle = shotAngle + (float)i / shotCount;
					tBullet.speedRate = 0;
					tBullet.angleRate = 0;
					tBullet.basicHP = bulletHp;
					tBullet.hp = bulletHp;
					tBullet.money = bulletMoney;
				}
			}
			yield return new WaitForSeconds (shotDelay);
		}
	}
}
