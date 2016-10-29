using UnityEngine;
using System.Collections;

public class Boss1 : MonoBehaviour {

	IntervalMultipleSpiralShooter interMul;
	RandomSpreadingShooter randSpreading;
	public Sprite Normal;
	public Sprite Red;
	SpriteRenderer sR;
	bool isNormal;

	void Awake()
	{
		sR = GetComponent<SpriteRenderer> ();
		interMul = GetComponent<IntervalMultipleSpiralShooter> ();
		randSpreading = GetComponent<RandomSpreadingShooter> ();
	}

	void LevelUp()
	{
		randSpreading.ShotCount = 100;
		interMul.shotCount = 100;
	}

	void Start () {
		//LevelUp ();
	}

	IEnumerator TrensformManage() {
		while(true)
		{
			if (isNormal) {
				sR.sprite = Red;
			} else {
				sR.sprite = Normal;
			}
			yield return new WaitForSeconds(30f);
		}
	}

}
