using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;

    float rad;
    Rigidbody2D ri;


    void Awake()
	{
		ri = GetComponent<Rigidbody2D> ();
	}

	void onEnable()
	{
		angle = 0;
		angleRate = 0;
		speed = 0;
		speedRate = 0;
	}

    void FixedUpdate()
    {
        rad = angle * Mathf.PI * 2;
        //ri.AddForce(new Vector3((speed * Mathf.Cos(rad)), (speed * Mathf.Sin(rad)), 0));
        tr.position += new Vector3((speed * Mathf.Cos(rad) * Time.deltaTime ), (speed * Mathf.Sin(rad) * Time.deltaTime), 0);
        angle += angleRate;
        speed += speedRate;
    }

}
