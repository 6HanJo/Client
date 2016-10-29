using UnityEngine;
using System.Collections;

public class SkillAngryAttacking : MonoBehaviour {

    public status skillset;
    public float shotCnt, intervalTime, shotIntervalTime, bulletSpeed;

    public DirectionalShooter[] shots;

    void Start()
    {
        StartCoroutine("Delay");
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
        for (int i = 0; i < shots.Length; i+=2)
        {
            shots[i].canShoot = true;
            shots[i].skill = true;
            shots[i].shotDelay = shotIntervalTime;
            shots[i].shotSpeed = bulletSpeed;
            shots[i].bulletStandTime = skillset.distance;
            shots[i].bulletDmg = skillset.damage;
            shots[i].bulletCnt = shotCnt;
            shots[i].gameObject.SetActive(true);
            shots[i+1].canShoot = true;
            shots[i+1].skill = true;
            shots[i+1].shotDelay = shotIntervalTime;
            shots[i+1].shotSpeed = bulletSpeed;
            shots[i+1].bulletStandTime = skillset.distance;
            shots[i+1].bulletDmg = skillset.damage;
            shots[i+1].bulletCnt = shotCnt;
            shots[i+1].gameObject.SetActive(true);
            yield return new WaitForSeconds(intervalTime);
            shots[i].gameObject.SetActive(false);
            shots[i + 1].gameObject.SetActive(false);
        }
        for (int i = shots.Length - 1 ; i > -1; i -= 2)
        {
            print("asd");
            shots[i].canShoot = true;
            shots[i].skill = true;
            shots[i].shotDelay = shotIntervalTime;
            shots[i].shotSpeed = bulletSpeed;
            shots[i].bulletStandTime = skillset.distance;
            shots[i].bulletDmg = skillset.damage;
            shots[i].bulletCnt = shotCnt;
            shots[i].gameObject.SetActive(true);
            shots[i - 1].canShoot = true;
            shots[i - 1].skill = true;
            shots[i - 1].shotDelay = shotIntervalTime;
            shots[i - 1].shotSpeed = bulletSpeed;
            shots[i - 1].bulletStandTime = skillset.distance;
            shots[i - 1].bulletDmg = skillset.damage;
            shots[i - 1].bulletCnt = shotCnt;
            shots[i - 1].gameObject.SetActive(true);
            yield return new WaitForSeconds(intervalTime);
            shots[i].gameObject.SetActive(false);
            shots[i - 1].gameObject.SetActive(false);
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
