using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;

    float rad;
    Transform tr;


    void Awake() {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        rad = angle * Mathf.PI * 2;
        tr.position += new Vector3((speed * Mathf.Cos(rad) * Time.deltaTime), (speed * Mathf.Sin(rad) * Time.deltaTime), 0);
        angle += angleRate;
        speed += speedRate;
    }
}
