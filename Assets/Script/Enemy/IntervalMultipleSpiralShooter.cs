using UnityEngine;
using System.Collections;

public class IntervalMultipleSpiralShooter : MonoBehaviour {
	public float shotAngle;
	public float shotAngleRate;
	public float shotSpeed;
	public float shotDelayTimer; 
	private float shotDelay;
	public int shotCount;
	public bool canShoot = false;
	public GameObject bullet;

	void Update () {
		shotDelay += Time.deltaTime;
		if (shotDelayTimer <= shotDelay && canShoot)
		{
			shotDelay = 0;
			shotAngle += shotCount * shotAngleRate;

			for (int i = 0; i < shotCount; i++)
			{
				GameObject tmp = Instantiate (bullet) as GameObject;
				Bullet tBullet = tmp.GetComponent<Bullet> ();
				tmp.transform.position = transform.position;
				tBullet.speed = shotSpeed;
				tBullet.angle = shotAngle + (float)i / shotCount;
			}


		}
	}
}
