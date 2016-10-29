using UnityEngine;
using System.Collections;

public class Boss2 : MonoBehaviour {
	OvertakingShooter[] overTaking = new OvertakingShooter[2];
	WavingNwayShooter wavingNway;
	RandomSpreadingShooter randomSpreading;
	GapShooter gap;
	BentSpiralShooter bentSpiral;
	bool isNormal = true;
	int patternCount = 0;

	void Awake() {
		overTaking = GetComponents<OvertakingShooter> ();
		wavingNway = GetComponent<WavingNwayShooter> ();
		randomSpreading = GetComponent<RandomSpreadingShooter> ();
		gap = GetComponent<GapShooter> ();
		bentSpiral = GetComponent<BentSpiralShooter> ();
	}

	void LevelUp()
	{
		if (isNormal && patternCount == 0) {
			gap.angleRange += 0.05f;
			++overTaking[0].ShotCount;
			bentSpiral.BulletAngleRate -= 0.0005f;
			bentSpiral.BulletSpeedRate += 0.003f;
			bentSpiral.ShotCount += 1;
		} else if (!isNormal) {
			++overTaking[0].ShotCount;
			randomSpreading.ShotCount += 5;
		} else {
			wavingNway.ShotCount += 1;
		}
	}

	void Start () {
		StartCoroutine (TrensformManage ());
	}

	IEnumerator ChangeFace() {
		int count = 0;
		while (count < 180) {
			count += 5;
			transform.rotation = Quaternion.Euler (0, 0, transform.rotation.z + count);
			yield return null;
		}
		transform.rotation = Quaternion.Euler (0, 0, 180);
	}

	IEnumerator ChangeNormal() {
		int count = 180;
		while (count > 0) {
			count -= 5;
			transform.rotation = Quaternion.Euler (0, 0, transform.rotation.z - count);
			yield return null;
		}
		transform.rotation = Quaternion.Euler (0, 0, 0);
	}

	IEnumerator TrensformManage() {
		while(true)
		{
			yield return new WaitForSeconds(15);
			if (isNormal && patternCount == 0) {
				gap.canShoot = false;
				overTaking[0].canShoot = false;
				bentSpiral.canShoot = false;
				overTaking [1].canShoot = true;
				randomSpreading.canShoot = true;
			} else if (isNormal && patternCount == 1) {
				StartCoroutine (ChangeFace ());
				isNormal = false;
				overTaking [1].canShoot = false;
				randomSpreading.canShoot = false;
				wavingNway.canShoot = true;
			}
			else {
				StartCoroutine (ChangeNormal ());
				isNormal = true;
				gap.canShoot = true;
				overTaking[0].canShoot = true;
				bentSpiral.canShoot = true;
				wavingNway.canShoot = false;
			}
			LevelUp ();
			++patternCount;
			if (patternCount >= 3)
				patternCount = 0;
		}
	}
}
