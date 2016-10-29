using UnityEngine;
using System.Collections;

public class Gulse : MonoBehaviour {

    public float dmg , range, movSpeed, waitTime, standTime;

    public bool moved = false;

    void OnEnabled()
    {
        StartCoroutine("Use");
    }

    void Start()
    {
        StartCoroutine("Use");
    }

    void Update()
    {
        if (moved)
        {
            transform.Translate(Vector3.right * movSpeed * Time.deltaTime);
        }
    }

    IEnumerator Use()
    {
        moved = true;
        transform.localScale = new Vector2(range, range);
        yield return new WaitForSeconds(waitTime);
        moved = false;
        //펑
        yield return new WaitForSeconds(standTime);
        Destroy(transform.root.gameObject);
        //transform.root.gameObject.SetActive(false);
    }

    void Bump() {
        //터짐 
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Boss")) {
            //보스 데미지
            StopCoroutine("Use");
            Bump();
        }
        else if (col.CompareTag("Bullet_Boss"))
        {
            if (col.GetComponent<Bullet>() == null)
                col.GetComponent<PlacedBullet>().HpManager(-dmg);
            else
                col.GetComponent<Bullet>().HpManager(-dmg);
        }
    }
}
