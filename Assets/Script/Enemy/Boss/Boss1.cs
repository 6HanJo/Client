using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour {

	IntervalMultipleSpiralShooter interMul;
	RandomSpreadingShooter randSpreading;
	BentSpiralShooter[] bentSpiral = new BentSpiralShooter[2];
	PlacedMultipleSpiralShooter placedMul;
	OvertakingShooter overTaking;

	public Sprite Normal;
	public Sprite Red;
	SpriteRenderer sR;
	bool isNormal = true;
	int patternCount = 0;

	void Awake()
	{
		sR = GetComponent<SpriteRenderer> ();
		interMul = GetComponent<IntervalMultipleSpiralShooter> ();
		randSpreading = GetComponent<RandomSpreadingShooter> ();
		bentSpiral = GetComponents<BentSpiralShooter> ();
		overTaking = GetComponent<OvertakingShooter> ();
		placedMul = GetComponent<PlacedMultipleSpiralShooter> ();
	}

	void LevelUp()
	{
		if (isNormal && patternCount == 0) {
			interMul.shotCount++;
			randSpreading.ShotCount += 10;
		} else if (!isNormal) {
			overTaking.GroupCount++;
		} else {
			for(int i = 0; i < 2; i++)
			{
				bentSpiral [i].ShotCount += 2;
				if(i == 0)
					bentSpiral [i].BulletAngleRate += 0.0005f;
				else 
					bentSpiral [i].BulletAngleRate -= 0.0005f;
				bentSpiral [i].BulletSpeedRate += 0.007f;
			}
		}
	}

	void Start () {
		StartCoroutine (TrensformManage ());
	}

	IEnumerator TrensformManage() {
		while(true)
		{
			yield return new WaitForSeconds(15);
			if (isNormal && patternCount == 0) {
				placedMul.canShoot = true;
				overTaking.canShoot = true;
				interMul.canShoot = false;
				randSpreading.canShoot = false;

			} else if (isNormal && patternCount == 1) {
				isNormal = false;
				sR.sprite = Red;
				bentSpiral [0].canShoot = true;
				bentSpiral [1].canShoot = true;
				placedMul.canShoot = false;
				overTaking.canShoot = false;
			}
			else {
				isNormal = true;
				sR.sprite = Normal;
				bentSpiral [0].canShoot = false;
				bentSpiral [1].canShoot = false;
				interMul.canShoot = true;
				randSpreading.canShoot = true;
			}
			LevelUp ();
			patternCount++;
			if (patternCount >= 3)
				patternCount = 0;
		}
	}

}
