using UnityEngine;
using System.Collections;

public class Skill2GusleMaker : MonoBehaviour {

    public status skillset;
    public float movSpeed, waitTime, standTime;

    public GameObject gusle;

    GameObject tmp;
    Gulse ltmp;

    void Start()
    {
        //StartCoroutine("Delay");
        UseSkill();
    }

    public void SetSkill(float damage, float range, float reload, float movSpeed, float waitTime, float standTime)
    {
        skillset.damage = damage;
        skillset.distance = range;
        skillset.reload = reload;
        this.movSpeed = movSpeed;
        this.waitTime = waitTime;
        this.standTime = standTime;
    }

    public void UseSkill()
    {
        for (int i = 0; i < 2; i++) {
            tmp = Instantiate(gusle, transform.position, Quaternion.Euler(0,0,180 * i)) as GameObject;
            ltmp = tmp.GetComponent<Gulse>();
            ltmp.dmg = skillset.damage;
            ltmp.range = skillset.distance;
            ltmp.movSpeed = movSpeed;
            ltmp.waitTime = waitTime;
            ltmp.standTime = standTime;
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
