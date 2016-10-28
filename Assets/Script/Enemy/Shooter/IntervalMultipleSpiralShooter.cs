using UnityEngine;
using System.Collections;
using PathologicalGames;

public class IntervalMultipleSpiralShooter : MonoBehaviour {
	public float shotAngle;
	public float shotAngleRate;
	public float shotSpeed;
	public float shotDelayTimer; 
	private float shotDelay;
	public int shotCount;
	public bool canShoot = false;
	public GameObject bullet;


    static SpawnPool spawnPool = null;
    Transform tmp;
    Bullet tBullet;

    void Start()
    {
        if (spawnPool == null)
        {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

    void Update () {
		if (canShoot) {
            StartCoroutine("SpawnObject");
		}
	}

    IEnumerator SpawnObject()
    {
        canShoot = false;
        shotAngle += shotCount * shotAngleRate;
        for (int i = 0; i < shotCount; i++)
        {
            tmp = spawnPool.Spawn(bullet, Vector3.zero, Quaternion.identity);
            tBullet = tmp.GetComponent<Bullet>();
            tmp.transform.position = transform.position;
            tBullet.speed = shotSpeed;
            tBullet.angle = shotAngle + (float)i / shotCount;
        }
        yield return new WaitForSeconds(shotDelay);
        canShoot = true;
    }


}
