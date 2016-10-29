using UnityEngine;
using System.Collections;

public class GapShooter : MonoBehaviour {
	public float angleRange;
	public float speed;
	public int count;
	public int Interval;
	private int Timer;
	public GameObject bullet;
	public bool canShoot = true;
	public float bulletMoney;
	public float bulletHp;
	void Update () {
		if (Timer == 0 && canShoot)
		{
			EnemyLib.instance.ShootNWay(
				transform.position, 
				Random.Range(0.0f, 1f), 
				angleRange, speed, count, 0, 0, bullet, bulletMoney, bulletHp);
		}
		if (canShoot)
			Timer = (Timer + 1) % Interval;
		if (!canShoot)
			Timer = 0;
	}
}
