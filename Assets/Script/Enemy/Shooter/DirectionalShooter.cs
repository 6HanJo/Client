using UnityEngine;
using System.Collections;
using PathologicalGames;

public class DirectionalShooter : MonoBehaviour {

	public float angle;
	public float shotSpeed;
	private float shotDelayTimer;
	public float shotDelay;
	public bool canShoot = true;
	public GameObject bullet;
	public int bulletMoney;
	public float bulletHp;
    public bool isPlayer = false, playerShoot = false, skill = false;

    public float bulletDmg, bulletCnt, bulletStandTime;

    static SpawnPool spawnPool = null;
    Transform tmp, tr;
    Bullet tBullet;
    BulletPlayer pBullet;

	void Awake() {
        tr = GetComponent<Transform>();
    }

    void Start() {
   
        if (spawnPool == null) {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

	void Update ()
    {
         if (skill && canShoot)
        {
            StartCoroutine("SpawnObject3");
        }
        else if(canShoot && !isPlayer)
        {
            StartCoroutine("SpawnObject");
        }
        else if (canShoot && isPlayer && !playerShoot)
        {
            StartCoroutine("SpawnObject2");
        }
        
	}

    IEnumerator SpawnObject()
    {
        canShoot = false;
        tmp = spawnPool.Spawn(bullet, Vector3.zero, Quaternion.identity);
        tBullet = tmp.GetComponent<Bullet>();
        tmp.transform.position = tr.position;
        tBullet.speed = shotSpeed;
        tBullet.angle = angle;
		tBullet.basicHP = bulletHp;
		tBullet.hp = bulletHp;
		tBullet.money = bulletMoney;
        yield return new WaitForSeconds(shotDelay);
        canShoot = true;
    }

    IEnumerator SpawnObject2()
    {
        canShoot = false;
        playerShoot = true;
        tmp = spawnPool.Spawn(bullet, PlayerControl.instance.bulletPos.localPosition, Quaternion.identity);
        pBullet = tmp.GetComponent<BulletPlayer>();
        tmp.transform.position = PlayerControl.instance.bulletPos.position;
        pBullet.speed = shotSpeed;
        pBullet.angle = angle;
        yield return new WaitForSeconds(shotDelay);
        playerShoot = false;
    }

    IEnumerator SpawnObject3()
    {
        canShoot = false;
        skill = false;
        for (int i = 0; i < bulletCnt; i++) {
            tmp = spawnPool.Spawn(bullet, transform.position, Quaternion.identity);
            pBullet = tmp.GetComponent<BulletPlayer>();
            tmp.transform.position = transform.position;
            pBullet.speed = shotSpeed;
            pBullet.angle = angle;
            pBullet.basicHP = bulletDmg;
            pBullet.GetComponent<SpawnCtrl>().SetTimeDespawn(bulletStandTime);
            yield return new WaitForSeconds(shotDelay);
        }
        gameObject.SetActive(false);
    }


}
