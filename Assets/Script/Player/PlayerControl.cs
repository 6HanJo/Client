using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
<<<<<<< HEAD
    public static PlayerControl instance;
=======
	public static PlayerControl instance;
>>>>>>> 433a9e4547b1ee5af15f9dd37f7078ce877bcedc

    //이동을 제어합니다.
    public float h, v;
    public bool inputed = true;

    //이동 속도
    public float basicSpeed, moveSpeed;

    //총알 정보
    public GameObject bullet;
    public Transform bulletPos;


    Transform tr;
    Rigidbody2D ri;
    //Animator ani;

    void Awake()
    {
<<<<<<< HEAD
        if (instance == null)
        {
            instance = this;
        }

=======
		if (instance == null)
			instance = this;
<<<<<<< HEAD
	
=======
>>>>>>> 59b65c345b83ee98fec2573ecd977030eb76d6db
>>>>>>> 433a9e4547b1ee5af15f9dd37f7078ce877bcedc
        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody2D>();
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
            else {
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
            else {
                v = 0;
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
        tr.position += new Vector3(v * moveSpeed * Time.deltaTime ,h * moveSpeed * Time.deltaTime, 0);
    }


    public void SetInputed(bool b)
    {
        inputed = b;
    }

}