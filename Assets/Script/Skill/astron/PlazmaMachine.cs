using UnityEngine;
using System.Collections;

public class PlazmaMachine : MonoBehaviour {

    public float hp, length, rotSpeed, waitTime, standTime;

    public bool roted = false;

    void OnEnabled()
    {
        StartCoroutine("Use");
    }

    void Start() {
        StartCoroutine("Use");
    }

    void HpManager(float num)
    {
        hp += num;
        if (hp <= 0)
        {
            transform.root.gameObject.SetActive(false);
        }
    }


    void Update() {
        if (roted) {
            transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        }
    }

    IEnumerator Use()
    {
        transform.localScale = new Vector2(1, length);
        yield return new WaitForSeconds(waitTime);
        roted = true;
        yield return new WaitForSeconds(standTime);
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
