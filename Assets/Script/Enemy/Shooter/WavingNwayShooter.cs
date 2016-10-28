﻿using UnityEngine;
using System.Collections;

public class WavingNwayShooter : MonoBehaviour {
	public float ShotAngle;
	public float ShotAngleRange;
	public float WavingAngleRange;
	public float ShotSpeed;
	public int ShotCount;
	public int Interval;
	public int Cycle;
	private int Timer;
	public bool canShoot = true;
	public GameObject bullet;

	void Update () 
	{
		if (Timer%Interval == 0 && canShoot)
		{
			EnemyLib.instance.ShootNWay(
				transform.position, 
				ShotAngle + WavingAngleRange * Mathf.Sin(Mathf.PI * 2 * Timer / Cycle), 
				ShotAngleRange, ShotSpeed, ShotCount, 0, 0, bullet);
		}
		if (canShoot)
			Timer = (Timer + 1) % Cycle;
		if (!canShoot)
			Timer = 0;
	}
}
