﻿using UnityEngine;
using System.Collections;

public class RollingNwayShooter : MonoBehaviour {
	public float ShotAngle;
	public float ShotAngleRange;
	public float ShotAngleRate;
	public float ShotSpeed;
	public int ShotCount;
	public int NWayCount;
	public int Interval;
	private int Timer;
	public bool canShoot = true;
	public GameObject bullet;
	public int bulletMoney;
	public float bulletHp;
	void Update () {
		if (Timer == 0 && canShoot)
		{
			for (int i = 0; i < NWayCount; i++)
			{
				EnemyLib.instance.ShootNWay(
					transform.position,
					ShotAngle + (float)i / NWayCount, ShotAngleRange, ShotSpeed,
					ShotCount, 0, 0, bullet, bulletMoney, bulletHp);
			}

			ShotAngle += ShotAngleRate;
			ShotAngle -= Mathf.Floor(ShotAngle);
		}
		Timer = (Timer + 1) % Interval;
	}
}
