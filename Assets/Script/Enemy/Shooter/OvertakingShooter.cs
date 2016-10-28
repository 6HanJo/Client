using UnityEngine;
using System.Collections;

public class OvertakingShooter : MonoBehaviour {
	public float ShotAngleRange;
	public float ShotSpeed;
	public int Interval;
	public float GroupSpeed;
	public float GroupAngle;
	public int ShotCount;
	public int GroupCount;
	public int GroupInterval;
	public float ShotAngle;
	private int Timer;
	public GameObject bullet;
	public bool canShoot = true;
	void Start () {
		ShotAngle = EnemyLib.instance.GetPlayerAngle (transform.position);
	}

	void Update () {
		int i = Timer / GroupInterval;

		if (canShoot)
		{
			if (i < GroupCount && Timer % GroupInterval == 0)
			{
				EnemyLib.instance.ShootNWay(
					transform.position,ShotAngle + GroupAngle * i, ShotAngleRange,
					ShotSpeed + GroupSpeed * i, ShotCount, 0, 0, bullet);
			}
			Timer = (Timer + 1) % Interval;
		}
		else 
			Timer = 0;
	}
}
