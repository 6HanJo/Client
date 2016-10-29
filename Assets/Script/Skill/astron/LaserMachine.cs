using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserMachine : MonoBehaviour
{

    public float hp, length, movSpeed, rotSpeed, waitTime, standTime;

    bool moved;

    Animator ani;

    void Start()
    {
        ani = transform.parent.GetComponent<Animator>();
        StartCoroutine("Use");
    }

    void OnEnabled()
    {
        StartCoroutine("Use");
    }


    void HpManager(float num) {
        hp += num;
        if (hp <= 0)
        {
            transform.root.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (moved)
        {
            transform.root.Translate(movSpeed * Time.deltaTime * Vector2.right);
            transform.parent.Rotate(Vector3.forward * rotSpeed);
        }
    }

    IEnumerator Use()
    {
        moved = true;
        yield return new WaitForSeconds(waitTime);
        moved = false;
        ani.SetBool("On", true);
        yield return new WaitForSeconds(standTime);
        ani.SetBool("On", false);
        Destroy(transform.root.gameObject);
        //transform.root.gameObject.SetActive(false);

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet_Boss"))
        {

            if (col.GetComponent<Bullet>() == null)
                col.GetComponent<PlacedBullet>().HpManager(-1000);
            else
                col.GetComponent<Bullet>().HpManager(-1000);
            HpManager(-1);
        }
    }
}

