using UnityEngine;
using System.Collections;

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
}
