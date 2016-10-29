using UnityEngine;
using System.Collections;

public class BossInfo : MonoBehaviour
{
    public float hp;

    public void HpManager(float num)
    {
        hp += num;

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet_Player"))
        {
            HpManager(-1);
        }
    }
}