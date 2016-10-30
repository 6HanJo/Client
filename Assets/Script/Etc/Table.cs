using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Table : MonoBehaviour
{
    public List<RotationLaser> rotationLaserTable;
    public List<CirclePlazma> circlePlazmaTable;
    public List<Marble> marbleTable;
    public List<ShoutAttack> shoutAttackTable;
    public List<HungryAttack> hungryAttackTable;
    public List<AngryAttack> angryAttackTable;

    public GameObject buyBtn;
    public GameObject atkBuyBtn;
    public GameObject lenBuyBtn;
    public GameObject coTBuyBtn;
    public int haveSkills,skillIndex,buildIndex,atkBuy,lenBuy,cotBuy;
    public int[] buyAddition;
    public int atkAdditionCnt;
    public int lenAdditionCnt;
    public int cotAdditionCnt;

    public void SkillSelect(int index)
    {
        skillIndex = index;
        int build = InGameManager.Instance.skillBuild[index-1];
        if (build == -1 &&  GameManager.Instance.totalGold >= buyAddition[haveSkills])
        {
            buyBtn.SetActive(true);
            buyBtn.transform.GetChild(0).GetComponent<Text>().text = "획득\n" + buyAddition[haveSkills] + "Gold";
        }
        else {
            buyBtn.SetActive(false);


            if (GameManager.Instance.totalGold >= atkBuy + (atkBuy * atkAdditionCnt)) {
                atkBuyBtn.SetActive(true);
                atkBuyBtn.transform.GetChild(0).GetComponent<Text>().text = "공격력 증가\n" +(atkBuy + (atkBuy * atkAdditionCnt))+"Gold";
            }
            else
                atkBuyBtn.SetActive(false);
            if (GameManager.Instance.totalGold >= lenBuy + (lenBuy * lenAdditionCnt))
            {
                lenBuyBtn.SetActive(true);
                lenBuyBtn.transform.GetChild(0).GetComponent<Text>().text = "사거리 증가\n" + lenBuy + (lenBuy * lenAdditionCnt) + "Gold";
            }
            else
                lenBuyBtn.SetActive(false);
            if (GameManager.Instance.totalGold >= cotBuy + (cotBuy * cotAdditionCnt))
            {
                coTBuyBtn.SetActive(true);
                coTBuyBtn.transform.GetChild(0).GetComponent<Text>().text = "쿨타임 감소\n" + cotBuy + (cotBuy * cotAdditionCnt) + "Gold";
            }
            else
                coTBuyBtn.SetActive(false);

        }
    }


    public void Buy() {
        ++InGameManager.Instance.skillBuild[skillIndex];
        GameManager.Instance.totalGold -= buyAddition[haveSkills];
        SkillSelect(skillIndex);
    }

    public void AtkBuy()
    {
        ++atkAdditionCnt;
        GameManager.Instance.totalGold -= atkBuy + (atkBuy * atkAdditionCnt);
        switch (skillIndex)
        {
            case 1:
                ++ShoutAttack.atkBuild;
                break;
            case 2:
                ++Marble.atkBuild;
                break;
            case 3:
                ++HungryAttack.atkBuild;
                break;
            case 4:
                ++AngryAttack.atkBuild;
                break;
            case 5:
                ++CirclePlazma.atkBuild;
                break;
            case 6:
                ++RotationLaser.atkBuild;
                break;
            default:
                break;
        }
        SkillSelect(skillIndex);
    }

    public void LenBuy()
    {
        ++lenAdditionCnt;
        GameManager.Instance.totalGold -= lenBuy + (lenBuy * lenAdditionCnt);
        switch (skillIndex)
        {
            case 1:
                ++ShoutAttack.lenBuild;
                break;
            case 2:
                ++Marble.lenBuild;
                break;
            case 3:
                ++HungryAttack.lenBuild;
                break;
            case 4:
                ++AngryAttack.lenBuild;
                break;
            case 5:
                ++CirclePlazma.lenBuild;
                break;
            case 6:
                ++RotationLaser.lenBuild;
                break;
            default:
                break;
        }
        SkillSelect(skillIndex);
    }

    public void CotBuy()
    {
        ++cotAdditionCnt;
        GameManager.Instance.totalGold -= cotBuy + (cotBuy * cotAdditionCnt);
        switch (skillIndex)
        {
            case 1:
                ++ShoutAttack.cotBuild;
                break;
            case 2:
                ++Marble.cotBuild;
                break;
            case 3:
                ++HungryAttack.cotBuild;
                break;
            case 4:
                ++AngryAttack.cotBuild;
                break;
            case 5:
                ++CirclePlazma.cotBuild;
                break;
            case 6:
                ++RotationLaser.cotBuild;
                break;
            default:
                break;
        }
        SkillSelect(skillIndex);
    }


}


[System.Serializable]
public class RotationLaser
{
    public float hp, length, reload, movSpeed, minRot, maxRot, waitTime, standTime;
    public int machineCnt;
    public static int buyMoney,buildMoney, atkBuild,lenBuild,cotBuild;



}

[System.Serializable]
public class CirclePlazma
{
    public float hp, length, reload, rotSpeed, waitTime, standTime;
    public static int buyMoney, buildMoney, atkBuild, lenBuild, cotBuild;

}

[System.Serializable]
public class Marble
{
    public float damage, range, reload, movSpeed, waitTime, standTime;
    public static int buyMoney, buildMoney, atkBuild, lenBuild, cotBuild;

}

[System.Serializable]
public class ShoutAttack
{
    public float damage, range, reload, ShoutDelay;
    public static int buyMoney, buildMoney, atkBuild, lenBuild, cotBuild;
}

[System.Serializable]
public class AngryAttack
{
    public float damage,  reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public static int buyMoney, buildMoney, atkBuild, lenBuild, cotBuild;
}

[System.Serializable]
public class HungryAttack
{
    public float damage, reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public static int buyMoney, buildMoney, atkBuild, lenBuild, cotBuild;
}
