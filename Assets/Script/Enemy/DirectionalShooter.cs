using UnityEngine;
using System.Collections;

public class DirectionalShooter : MonoBehaviour {

	public float angle;
	public float shotSpeed;
	public float shotDelayTimer;
	public float shotDelay;
	public bool canShoot = true;
	public GameObject bullet;

	void Update () {
		shotDelayTimer += Time.deltaTime;

		if (shotDelayTimer >= shotDelay && canShoot) {
			Instantiate (bullet);
		}
	}	
}
