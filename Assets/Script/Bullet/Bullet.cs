using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public float basicHP, hp;
	public int money;
    public float angle = 0;
    public float angleRate = 0;
    public float speed = 0;
    public float speedRate = 0;

    float rad;

    Transform tr;
    SpawnCtrl sc;

    void Awake()
	{
        tr = GetComponent<Transform>();
        sc = GetComponent<SpawnCtrl>();
    }
    void OnEnable()
    {
        hp = basicHP;
    }
    public void HpManager(float num)
    {
        hp += num;
        if (hp <= 0)
        {
            sc.SetActives();
			GameManager.Instance.totalGold += money;
            UIManager.Instance.SetTextTotalGold(GameManager.Instance.totalGold);
			print (GameManager.Instance.totalGold);
        }
        float percent = (hp / basicHP) * 100;

        if (percent > 0)
        {
            tr.localScale = new Vector3(1 * percent / 100, 1 * percent / 100, 1);
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
        tr.position += new Vector3((speed * Mathf.Cos(rad) * Time.deltaTime ), (speed * Mathf.Sin(rad) * Time.deltaTime), 0);
        angle += angleRate;
        speed += speedRate;
    }

}
