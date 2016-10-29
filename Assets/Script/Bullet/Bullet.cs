using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float basicHP, hp;

    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;

    float rad;

    Transform tr;
    Rigidbody2D ri;
    SpawnCtrl sc;

    void Awake()
	{
        tr = GetComponent<Transform>();
		ri = GetComponent<Rigidbody2D> ();
        sc = GetComponent<SpawnCtrl>();
    }
    void OnEnable()
    {
        hp = basicHP;
    }
    public void HpManager(int num)
    {
        hp += num;
        if (hp < 0)
        {
            sc.SetActives();
        }
    }

	void onEnable()
	{
		angle = 0;
		angleRate = 0;
		speed = 0;
		speedRate = 0;
	}

    void Update()
    {
        rad = angle * Mathf.PI * 2;
        //ri.AddForce(new Vector3((speed * Mathf.Cos(rad)), (speed * Mathf.Sin(rad)), 0));
        tr.position += new Vector3((speed * Mathf.Cos(rad) * Time.deltaTime ), (speed * Mathf.Sin(rad) * Time.deltaTime), 0);
        angle += angleRate;
        speed += speedRate;
    }

}
