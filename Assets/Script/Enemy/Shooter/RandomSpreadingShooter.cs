using UnityEngine;
using System.Collections;

public class RandomSpreadingShooter : MonoBehaviour {
	public float ShotAngleRange;
	public float ShotSpeed;
	public float ShotSpeedRange;
	public int ShotCount;
	public int Interval;
	private int Timer;
	public bool canShoot = true;
	public GameObject bullet;

	void Update () {
		if (Timer == 0 && canShoot)
		{
			for (int i = 0; i < ShotCount; i++)
			{
				GameObject tmp = Instantiate (bullet) as GameObject;
				Bullet tBullet = tmp.GetComponent<Bullet> ();
				tmp.transform.position = transform.position;
				tBullet.speed = ShotSpeed + ShotSpeedRange * (Random.Range(0.0f, 1.0f) - 0.5f);
				tBullet.angle = EnemyLib.instance.GetPlayerAngle (transform.position) + ShotAngleRange * (Random.Range (0.0f, 1.0f) - 0.5f);
			}
		}
		if (canShoot)
			Timer = (Timer + 1) % Interval;
		if (!canShoot)
			Timer = 0;
	}
}
