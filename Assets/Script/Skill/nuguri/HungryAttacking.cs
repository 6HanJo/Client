using UnityEngine;
using System.Collections;

public class HungryAttacking : MonoBehaviour
{

    public status skillset;
    public float shotCnt, intervalTime, shotIntervalTime, bulletSpeed;

    public DirectionalShooter[] shots;

    void Start()
    {
        //StartCoroutine("Delay");
        UseSkill();
    }

    public void SetSkill(float damage, float reload, float bulletCnt, float bulletStandTime, float bulletSpeed, float skillIntervalTime, float shotIntervalTime)
    {
        skillset.damage = damage;
        skillset.distance = bulletStandTime;
        skillset.reload = reload;
        this.shotCnt = bulletCnt;
        this.intervalTime = skillIntervalTime;
        this.shotIntervalTime = shotIntervalTime;
        this.bulletSpeed = bulletSpeed;
    }

    public void UseSkill()
    {
        StartCoroutine("Shot");
    }

    IEnumerator Shot()
    {
        for (int i = 0; i < shots.Length; i++)
        {
            shots[i].canShoot = true;
            shots[i].skill = true;
            shots[i].shotDelay = shotIntervalTime;
            shots[i].shotSpeed = bulletSpeed;
            shots[i].bulletStandTime = skillset.distance;
            shots[i].bulletDmg = skillset.damage;
            shots[i].bulletCnt = shotCnt;
            shots[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(intervalTime);
        }
    }


    IEnumerator Delay()
    {
        while (true)
        {
            yield return new WaitForSeconds(skillset.reload);
            UseSkill();
        }
    }

}
