using UnityEngine;
using System.Collections;

public class Shout : MonoBehaviour {

    public float dmg, range;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet_Boss"))
        {
            if (col.GetComponent<Bullet>() == null)
                col.GetComponent<PlacedBullet>().HpManager(-1000);
            else
                col.GetComponent<Bullet>().HpManager(-1000);
        }
    }
}
