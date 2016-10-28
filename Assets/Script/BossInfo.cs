using UnityEngine;
using System.Collections;

public class BossInfo : MonoBehaviour {

    public float hp;

    void HpManager(int num) {
        hp += num;
        print(hp);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Bullet")) {
            HpManager(-1);
        }
    }
}
