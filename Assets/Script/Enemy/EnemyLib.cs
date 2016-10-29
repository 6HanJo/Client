using UnityEngine;
using System.Collections;
using PathologicalGames;



public class EnemyLib : MonoBehaviour {
	public static EnemyLib instance;

    static SpawnPool spawnPool = null;
    Transform tmp, tr;
    Bullet tBullet;

    void Awake()
	{
		if (instance == null)
			instance = this;
	}

    void Start() {
        if (spawnPool == null)
        {
            spawnPool = PoolManager.Pools["Test"];
        }
    }

	public float GetPlayerAngle(Vector2 pos)
	{
		return Mathf.Atan2(PlayerControl.instance.transform.position.y - pos.y,PlayerControl.instance.transform.position.x - pos.x) / Mathf.PI / 2;
	} 

	public void ShootNWay(Vector2 pos, float angle, float angleRange, float speed, int count, float angleRate, float speedRate, GameObject bullet)
	{
		for (int i = 0; i < count; i++) {
            tmp = spawnPool.Spawn(bullet);
            Bullet tBullet = tmp.GetComponent<Bullet> ();
			tmp.transform.position = pos;
			tBullet.speed = speed;
			tBullet.angle = angle + angleRange * ((float)i / (count - 1) - 0.5f);
			tBullet.angleRate = angleRate;
			tBullet.speedRate = speedRate;
		}
	}

	public void ShootPlacedNWay(Vector2 pos, float angle, float angleRange, float speed, int count, int moveTime, int stopTime, GameObject bullet)
	{
		for (int i = 0; i < count; i++) {
            tmp = spawnPool.Spawn(bullet);
            PlacedBullet tBullet = tmp.GetComponent<PlacedBullet> ();
			tmp.transform.position = pos;
			tBullet.angle = angle + angleRange * ((float)i / (count - 1) - 0.5f);
			tBullet.speed = speed;
			tBullet.InitialSpeed = speed;
			tBullet.MoveTime = moveTime;
			tBullet.StopTime = stopTime;
			tBullet.speedRate = 0;
			tBullet.angleRate = 0;
		}		
	}
}
