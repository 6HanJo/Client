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
	public int bulletMoney;
	public float bulletHp;
    Transform tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }
		
	void Update () {
		int i = Timer / GroupInterval;

		if (canShoot)
		{
			if (Timer == 0) {
				ShotAngle = EnemyLib.instance.GetPlayerAngle (tr.position);
				GroupAngle *= -1;
			}if (i < GroupCount && Timer % GroupInterval == 0)
			{
				EnemyLib.instance.ShootNWay(
					tr.position,ShotAngle + GroupAngle * i, ShotAngleRange,
					ShotSpeed + GroupSpeed * i, ShotCount, 0, 0, bullet, bulletMoney, bulletHp);
			}
			Timer = (Timer + 1) % Interval;
		}
		else 
			Timer = 0;
	}
}
