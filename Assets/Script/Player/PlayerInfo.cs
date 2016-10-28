using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    public float hp;

    void HpManager(float num)
    {
        hp += num;
        print(hp);
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet_Boss"))
        {
            HpManager(-1);
        }
    }
}
