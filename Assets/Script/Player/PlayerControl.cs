using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	public static PlayerControl instance;

    public static PlayerControl instance;

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
			instance = this;
		
=======
        if (instance == null) {
            instance = this;
        }

>>>>>>> ee8113153a8c537cc4227019a9f2177050ef2e8d
        tr = GetComponent<Transform>();
        ri = GetComponent<Rigidbody2D>();
        //ani = GetComponent<Animator>();
    }


    void Update()
    {
        if (inputed)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (Input.GetKey(KeyCode.Space))
            {
                Instantiate(bullet, bulletPos.position, Quaternion.identity);
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
        ri.velocity = new Vector2(h * moveSpeed, v * moveSpeed);
    }


    public void SetInputed(bool b)
    {
        inputed = b;
    }

}