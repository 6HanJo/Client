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

    public bool isPlayer = false, playerShoot = false;

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

	void Update () {
        if (canShoot && !isPlayer) {
            StartCoroutine("SpawnObject");
        }
        else if (canShoot && isPlayer && !playerShoot) {
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


}
