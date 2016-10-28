using UnityEngine;
using System.Collections;

public class BossInfo : MonoBehaviour
{

    public float hp;

    void HpManager(float num)
    {
        hp += num;
        print(hp);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet_Player"))
        {
            HpManager(-1);
        }
    }
}