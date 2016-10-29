using UnityEngine;
using System.Collections;

public class PlacedBulletPlayer : MonoBehaviour
{
    public float basicHP,hp;

    public float InitialSpeed;
    public int MoveTime;
    public int StopTime;
    public int Timer;
    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;


    Transform tr;
    SpawnCtrl sc;

    void Awake()
    {
        tr = GetComponent<Transform>();
        sc = GetComponent<SpawnCtrl>();
    }

    void OnEnable()
    {
        hp = basicHP;
    }

    public void HpManager(float num)
    {
        hp += num;
        if (hp <= 0)
        {
            sc.SetActives();
        }
        float percent = (hp / basicHP) * 100;
        tr.localScale = new Vector3(1 * percent / 100, 1 * percent / 100, 1);

    }

    void Update()
    {
        if (Timer == MoveTime)
            speed = 0;

        if (Timer == MoveTime + StopTime)
            speed = InitialSpeed;

        Timer++;

        float rad = angle * Mathf.PI * 2;

        tr.position += new Vector3((speed * Mathf.Cos(rad) * Time.deltaTime), speed * Mathf.Sin(rad) * Time.deltaTime, 0);

        angle += angleRate;
        speed += speedRate;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Contains("Bullet") && (gameObject.tag != col.tag))
        {
            if (col.GetComponent<Bullet>() == null)
                col.GetComponent<PlacedBullet>().HpManager(-hp);
            else
                col.GetComponent<Bullet>().HpManager(-hp);
            HpManager(-hp);
        }
    }

}
