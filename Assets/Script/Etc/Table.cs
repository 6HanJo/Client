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

    public GameObject popInfo;


    public void SkillSelect(int index)
    {
        int build = InGameManager.Instance.skillBuild[index];
        if (build == -1)
        {

        }
    }


            /*
            switch (index) {
                case 1:
                    if (GameManager.Instance.totalGold > ShoutAttack.buyMoney)
                    {

                    }
                    break;
                case 2:
                    if (GameManager.Instance.totalGold > Marble.buyMoney)
                    {
                        skill
                    }
                    break;
                case 3:
                    if (GameManager.Instance.totalGold > HungryAttack.buyMoney)
                    {
                        skill
                    }
                    break;
                case 4:
                    if (GameManager.Instance.totalGold > AngryAttack.buyMoney)
                    {

                    }
                    break;
                case 5:
                    if (GameManager.Instance.totalGold > RotationLaser.buyMoney)
                    {
                        skill
                    }
                    break;
                case 6:
                    if (GameManager.Instance.totalGold > CirclePlazma.buyMoney)
                    {

                    }
                    break;
                default: break;
            }
        }
        else {

        }

    }
    */





}


[System.Serializable]
public class RotationLaser
{
    public float hp, length, reload, movSpeed, minRot, maxRot, waitTime, standTime;
    public int machineCnt;
    public static int buyMoney,buildMoney;



}

[System.Serializable]
public class CirclePlazma
{
    public float hp, length, reload, rotSpeed, waitTime, standTime;
    public static int buyMoney, buildMoney;

}

[System.Serializable]
public class Marble
{
    public float damage, range, reload, movSpeed, waitTime, standTime;
    public static int buyMoney, buildMoney;

}

[System.Serializable]
public class ShoutAttack
{
    public float damage, range, reload, ShoutDelay;
    public static int buyMoney, buildMoney;
}

[System.Serializable]
public class AngryAttack
{
    public float damage,  reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public static int buyMoney, buildMoney;
}

[System.Serializable]
public class HungryAttack
{
    public float damage, reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public static int buyMoney, buildMoney;
}
