using UnityEngine;
using System.Collections;

public class Skill5PlazmaMaker : MonoBehaviour
{


    public status skillset;
    public float rotSpeed, waitTime, standTime;

    public GameObject machine;


    GameObject tmp;
    PlazmaMachine ltmp;

    void Start()
    {
        //StartCoroutine("Delay");
        //UseSkill();
    }

    public void SetSkill(float hp, float length, float reload, float rotSpeed, float waitTime, float standTime)
    {
        skillset.damage = hp;
        skillset.distance = length;
        skillset.reload = reload;
        this.rotSpeed = rotSpeed;
        this.waitTime = waitTime;
        this.standTime = standTime;
    }

    public void UseSkill()
    {
        tmp = Instantiate(machine, transform.position, Quaternion.identity) as GameObject;
        ltmp = tmp.transform.GetChild(0).GetComponent<PlazmaMachine>();
        ltmp.hp = skillset.damage;
        ltmp.length = skillset.distance;
        ltmp.rotSpeed = rotSpeed;
        ltmp.waitTime = waitTime;
        ltmp.standTime = standTime;
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
