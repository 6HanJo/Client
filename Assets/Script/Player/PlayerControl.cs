using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

    public static PlayerControl instance;

    //이동을 제어합니다.
    public float h, v;
    public bool inputed = true;

    //이동 속도
    public float basicSpeed, moveSpeed;

    //총알 정보
    public Transform bulletPos;


    DirectionalShooter dr;

    Transform tr;
    Rigidbody2D ri;
    //Animator ani;

    void Awake()
    {
        if (instance == null)
            instance = this;


        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody2D>();
        dr = GetComponent<DirectionalShooter>();
        //ani = GetComponent<Animator>();
    }


    void Update()
    {
        if (inputed)
        {
            //4방향 키를 기준을 이동합니다.
            if (Input.GetKey(KeyCode.W))
            {
                h = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                h = -1;
            }
            else
            {
                h = 0;
            }
            if (Input.GetKey(KeyCode.A))
            {
                v = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                v = 1;
            }
            else
            {
                v = 0;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                dr.canShoot = true;
            }
        }
    }


    void FixedUpdate()
    {
        if (inputed)
        {
            Move();
        }
    }


    void Move()
    {
        tr.position += new Vector3(v * moveSpeed * Time.deltaTime, h * moveSpeed * Time.deltaTime, 0);
    }


    public void SetInputed(bool b)
    {
        inputed = b;
    }

}