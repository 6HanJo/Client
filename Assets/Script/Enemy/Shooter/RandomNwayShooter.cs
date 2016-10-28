using UnityEngine;
using System.Collections;

public class RandomNwayShooter : MonoBehaviour {
	public float ShotAngleRange;
	public float ShotSpeed;
	public int ShotCount;
	public int Interval;
	private int Timer;
	public GameObject bullet;
	public bool canShoot = true;

	void Update () {
		if (Timer == 0 && canShoot)
		{
			for (int i = 0; i < ShotCount; i++)
			{
				GameObject tmp = Instantiate (bullet) as GameObject;
				Bullet tBullet = tmp.GetComponent<Bullet> ();
				tmp.transform.position = transform.position;
				tBullet.speed = ShotSpeed;
				tBullet.angle = EnemyLib.instance.GetPlayerAngle (transform.position) + ShotAngleRange * (Random.Range (0.0f, 1.0f) - 0.5f);
			}
		}
		if (canShoot)
			Timer = (Timer + 1) % Interval;
		if (!canShoot)
			Timer = 0;
	}
}
