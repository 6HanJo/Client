using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//공격력 사거리 재장전
public class LaserRotation : MonoBehaviour
{

    public status skillset;

    public float movSpeed,minRot, maxRot, waitTime, standTime;
    public int machineCnt;
    public List<GameObject> machineList;



    public GameObject machine;

    GameObject tmp;
    LaserMachine ltmp;

    void Start()
    {
        StartCoroutine("Delay");
        UseSkill();
    }

    public void SetSkill(float hp, float length, float reload, float movSpeed, float minRot, float maxRot, float waitTime, float standTime, int machineCnt) {
        skillset.damage = hp;
        skillset.distance = length;
        skillset.reload = reload;
        this.movSpeed = movSpeed;
        this.minRot = minRot;
        this.maxRot = maxRot;
        this.waitTime = waitTime;
        this.standTime = standTime;
        this.machineCnt = machineCnt;
    }

    public void UseSkill()
    {
        float rad = (float)(360f / machineCnt);

        for (int i = 0; i < machineCnt; i++) {
            tmp = Instantiate(machine,transform.position,Quaternion.identity) as GameObject;
            tmp.transform.rotation = Quaternion.Euler(0,0, (rad * i) );
            machineList.Add(tmp);
            ltmp = tmp.transform.GetChild(0).transform.GetChild(0).GetComponent<LaserMachine>();
            ltmp.hp = skillset.damage;
            ltmp.length = skillset.distance;
            ltmp.movSpeed = movSpeed;
            ltmp.rotSpeed = Random.Range(minRot, maxRot);
            ltmp.waitTime = waitTime;
            ltmp.standTime = standTime;
        }

    }

    IEnumerator Delay() {
        while (true) {
            yield return new WaitForSeconds(skillset.reload);
            UseSkill();
        }
    }

}


