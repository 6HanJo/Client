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
    Rigidbody2D ri;


    void Awake()
    {
        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rad = angle * Mathf.PI * 2;
        ri.AddForce(new Vector3((speed * Mathf.Cos(rad)), (speed * Mathf.Sin(rad)), 0));
        angle += angleRate;
        speed += speedRate;
    }
}
