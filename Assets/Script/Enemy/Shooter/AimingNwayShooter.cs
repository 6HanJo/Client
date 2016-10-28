using UnityEngine;
using System.Collections;

public class AimingNwayShooter : MonoBehaviour {
	public float ShotAngleRange;
	public float ShotSpeed;
	public int Interval;
	private int Timer;
	public int ShotCount;
	public int MoveTime;
	public int StopTime;
	public bool canShoot = true;
	public GameObject bullet;

	void Update () {
		if (Timer == 0 && canShoot)
			EnemyLib.instance.ShootPlacedNWay(transform.position, EnemyLib.instance.GetPlayerAngle(transform.position), ShotAngleRange, ShotSpeed, ShotCount, MoveTime, StopTime, bullet);
		Timer = (Timer + 1) % Interval;
	}
}
