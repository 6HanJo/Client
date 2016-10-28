using UnityEngine;
using System.Collections;
using PathologicalGames;

public class DirectionalShooter : MonoBehaviour {

	public float angle;
	public float shotSpeed;
	private float shotDelayTimer;
	public float shotDelay;
	public bool canShoot = true;
	public GameObject bullet;

    static SpawnPool spawnPool = null;


    Transform tmp;
    Bullet tBullet;

    void Start() {
        if (spawnPool == null) {
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
        tmp = spawnPool.Spawn(bullet, Vector3.zero, Quaternion.identity);
        tBullet = tmp.GetComponent<Bullet>();
        tmp.transform.position = transform.position;
        tBullet.speed = shotSpeed;
        tBullet.angle = angle;
        yield return new WaitForSeconds(shotDelay);
        canShoot = true;
    }


}
