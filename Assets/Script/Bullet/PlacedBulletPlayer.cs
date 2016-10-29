﻿using UnityEngine;
using System.Collections;

public class PlacedBulletPlayer : MonoBehaviour
{
    public float InitialSpeed;
    public int MoveTime;
    public int StopTime;
    public int Timer;
    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;


    Transform tr;
    Rigidbody2D ri;


    void Awake()
    {
        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody2D>();
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
        //ri.AddForce(new Vector3(speed * Mathf.Cos(rad) * Time.deltaTime, speed * Mathf.Sin(rad) * Time.deltaTime,0));

        angle += angleRate;
        speed += speedRate;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Contains("Bullet") && (gameObject.tag != col.tag))
        {
            col.GetComponent<SpawnCtrl>().SetActives();
            gameObject.GetComponent<SpawnCtrl>().SetActives();
        }
    }

}