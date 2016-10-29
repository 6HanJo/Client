using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour {

	IntervalMultipleSpiralShooter interMul;
	RandomSpreadingShooter randSpreading;
	BentSpiralShooter[] bentSpiral = new BentSpiralShooter[2];
	public Sprite Normal;
	public Sprite Red;
	SpriteRenderer sR;
	bool isNormal = true;
	void Awake()
	{
		sR = GetComponent<SpriteRenderer> ();
		interMul = GetComponent<IntervalMultipleSpiralShooter> ();
		randSpreading = GetComponent<RandomSpreadingShooter> ();
		bentSpiral = GetComponents<BentSpiralShooter> ();
	}

	void LevelUp()
	{
		if (isNormal) {
			interMul.shotCount++;
			randSpreading.ShotCount += 10;
		} else {
			for(int i = 0; i < 2; i++)
			{
				bentSpiral [i].ShotCount += 4;
				if(i == 0)
					bentSpiral [i].BulletAngleRate += 0.001f;
				else 
					bentSpiral [i].BulletAngleRate -= 0.001f;
				bentSpiral [i].BulletSpeedRate += bentSpiral [i].BulletSpeedRate;
			}
		}
	}
	void Start () {
		StartCoroutine (TrensformManage ());
	}

	IEnumerator TrensformManage() {
		while(true)
		{
			yield return new WaitForSeconds(30);
			if (isNormal) {
				print ("Red");
				isNormal = false;
				sR.sprite = Red;
				bentSpiral [0].canShoot = true;
				bentSpiral [1].canShoot = true;
				interMul.canShoot = false;
				randSpreading.canShoot = false;
			} else {
				print ("Noraml");
				isNormal = true;
				sR.sprite = Normal;
				bentSpiral [0].canShoot = false;
				bentSpiral [1].canShoot = false;
				interMul.canShoot = true;
				randSpreading.canShoot = true;
			}
			LevelUp ();
		}
	}

}
