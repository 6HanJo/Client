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
    public GameObject bullet;
    public Transform bulletPos;


    Transform tr;
    Rigidbody2D ri;
    //Animator ani;

    void Awake()
    {
		if (instance == null)
			instance = this;
<<<<<<< HEAD
	
=======
>>>>>>> 59b65c345b83ee98fec2573ecd977030eb76d6db
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