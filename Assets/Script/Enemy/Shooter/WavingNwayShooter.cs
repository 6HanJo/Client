using UnityEngine;
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
	public int bulletMoney;
	public float bulletHp;
    Transform tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Update () 
	{
		if (Timer%Interval == 0 && canShoot)
		{
			EnemyLib.instance.ShootNWay(
				tr.position, 
				ShotAngle + WavingAngleRange * Mathf.Sin(Mathf.PI * 2 * Timer / Cycle), 
				ShotAngleRange, ShotSpeed, ShotCount, 0, 0, bullet, bulletMoney, bulletHp);
		}
		if (canShoot)
			Timer = (Timer + 1) % Cycle;
		if (!canShoot)
			Timer = 0;
	}
}
