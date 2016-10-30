using UnityEngine;
using System.Collections;

public class BossInfo : MonoBehaviour
{
	InGameManager inGameManager;
	UIManager uiManager;

    public int hp;

	void Start()
	{
		uiManager = UIManager.Instance;
		inGameManager = InGameManager.Instance;
		print (hp);
	}

    public void HpManager(int num)
    {
		inGameManager.bossHp += num;

		uiManager.SetTextBossHP (inGameManager.bossHp);
		uiManager.SetMaxSliderBossHP (inGameManager.bossHp);

		if (inGameManager.bossHp  <= 0)
			this.gameObject.SetActive (false);
	}
}