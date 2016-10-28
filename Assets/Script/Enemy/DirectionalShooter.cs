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

	void Update () {
		shotDelayTimer += Time.deltaTime;

		if (shotDelayTimer >= shotDelay && canShoot) {
			shotDelayTimer = 0;
			GameObject tmp = Instantiate (bullet) as GameObject;
			Bullet tBullet = tmp.GetComponent<Bullet> ();
			tmp.transform.position = transform.position;
			tBullet.speed = shotSpeed;
			tBullet.angle = angle;
		}
	}

    IEnumerator SpawnObject()
    {
        yield return new WaitForSeconds(shotDelay);

        PoolManager.Pools["Test"].Spawn(bullet, Vector3.zero, Quaternion.identity);

        print(PoolManager.Pools["Test"].name);
        Transform spawnInstance;
        SpawnPool spawnPool = PoolManager.Pools["Test"];
        spawnInstance = spawnPool.Spawn(bullet);

        Bullet tBullet = spawnInstance.GetComponent<Bullet>();
        spawnInstance.transform.position = transform.position;
        tBullet.speed = shotSpeed;
        tBullet.angle = angle;

        

    }


}
