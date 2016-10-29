using UnityEngine;
using System.Collections;

public class Boss3 : MonoBehaviour {
	SpriteRenderer sR;
	public Sprite Normal;
	public Sprite Red;
	RandomNwayShooter randNway;
	OvertakingShooter overTaking;
	WavingNwayShooter wavingNway;
	RollingNwayShooter rollingNway;
	RandomSpreadingShooter randomSpreading;
	GapShooter gap;
	PlacedMultipleSpiralShooter placedMulSpiral;
	PatternShooter patternS;
	bool isNormal = true;
	bool isStarted = false;
	int patternCount = 0;

	void Awake()
	{
		sR = GetComponent<SpriteRenderer> ();
		patternS = GetComponent<PatternShooter> ();
		randNway = GetComponent<RandomNwayShooter> ();
		overTaking = GetComponent<OvertakingShooter> ();
		wavingNway = GetComponent<WavingNwayShooter> ();
		randomSpreading = GetComponent<RandomSpreadingShooter> ();
		rollingNway = GetComponent<RollingNwayShooter> ();
		gap = GetComponent<GapShooter> ();
		placedMulSpiral = GetComponent<PlacedMultipleSpiralShooter> ();
	}

	void LevelUp()
	{
		if (isNormal && patternCount == 0) {
			++randNway.ShotCount;
		} else if (isNormal && patternCount == 1) {
			++wavingNway.ShotCount;
			randomSpreading.ShotCount += 5;
		} else if (!isNormal) {
			++overTaking.ShotCount;
			wavingNway.ShotCount += 1;
			rollingNway.ShotSpeed += 0.02f;
			randomSpreading.ShotCount += 5;
		}
	}

	void Update () {

		if (!isStarted && !patternS.canShoot) {
			StartCoroutine (TrensformManage ());
			isStarted = true;
		}
	}

	IEnumerator TrensformManage() {
		while (true) {
			if(!isNormal) {
				isNormal = true;
				sR.sprite = Normal;
				overTaking.canShoot = false;
				rollingNway.canShoot = false;
				randomSpreading.canShoot = false;
				patternS.canShoot = true;
				patternCount = 0;
			}
			if (isNormal && patternCount == 0) {
				randNway.canShoot = true;
				placedMulSpiral.canShoot = true;
			} else if (isNormal && patternCount == 1) {
				randNway.canShoot = false;
				placedMulSpiral.canShoot = false;
				gap.canShoot = true;
				wavingNway.canShoot = true;
			} else if (isNormal && patternCount == 2) {
				isNormal = false;
				sR.sprite = Red;
				gap.canShoot = false;
				wavingNway.canShoot = false;
				overTaking.canShoot = true;
				rollingNway.canShoot = true;
				randomSpreading.canShoot = true;
			}
			yield return new WaitForSeconds (15);
			LevelUp ();
			++patternCount;
		}
	}
	void Reset() {
		patternCount = 0;
	}
}
