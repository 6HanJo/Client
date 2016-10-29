using UnityEngine;
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



}


[System.Serializable]
public class RotationLaser
{
    public float hp, length, reload, movSpeed, minRot, maxRot, waitTime, standTime;
    public int machineCnt;

}

[System.Serializable]
public class CirclePlazma
{
    public float hp, length, reload, rotSpeed, waitTime, standTime;
    public int machineCnt;

}

[System.Serializable]
public class Marble
{
    public float damage, range, reload, movSpeed, waitTime, standTime;
    public int machineCnt;

}

[System.Serializable]
public class ShoutAttack
{
    public float damage, range, reload, ShoutDelay;
    public int machineCnt;

}

[System.Serializable]
public class AngryAttack
{
    public float damage,  reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public int machineCnt;

}

[System.Serializable]
public class HungryAttack
{
    public float damage, reload, bulletCnt, bulletStandTime, bulletSpeed, skillIntervalTime, shotIntervalTime;
    public int machineCnt;

}
