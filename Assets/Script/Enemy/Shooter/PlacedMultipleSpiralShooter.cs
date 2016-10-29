using UnityEngine;
using System.Collections;
using PathologicalGames;

public class PlacedMultipleSpiralShooter : MonoBehaviour {

	public float shotAngle;
	public float shotAngleRate;
	public float shotSpeed;
	public float shotDelay;
	public int shotCount;
	public bool canShoot = true;
	public int MoveTime;
	public int StopTime;
	public GameObject bullet;


	static SpawnPool spawnPool = null;
	Transform tmp, tr;
	PlacedBullet tBullet;

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
					tBullet = tmp.GetComponent<PlacedBullet> ();
					tmp.transform.position = tr.position;
					tBullet.speed = shotSpeed;
					tBullet.angle = shotAngle + (float)i / shotCount;
					tBullet.speedRate = 0;
					tBullet.angleRate = 0;
					tBullet.MoveTime = MoveTime;
					tBullet.StopTime = StopTime;
				}
			}
			yield return new WaitForSeconds (shotDelay);
		}
	}
}
