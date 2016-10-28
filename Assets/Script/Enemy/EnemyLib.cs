using UnityEngine;
using System.Collections;

public class EnemyLib : MonoBehaviour {
	public static EnemyLib instance;

	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public float GetPlayerAngle(Vector2 pos)
	{
		return Mathf.Atan2(PlayerControl.instance.transform.position.y - pos.y,PlayerControl.instance.transform.position.x - pos.x) / Mathf.PI / 2;
	} 

	public void ShootNWay(Vector2 pos, float angle, float angleRange, float speed, int count, float angleRate, float speedRate, GameObject bullet)
	{
		for (int i = 0; i < count; i++) {
			GameObject tmp = Instantiate (bullet) as GameObject;
			Bullet tBullet = tmp.GetComponent<Bullet> ();
			tmp.transform.position = pos;
			tBullet.speed = speed;
			tBullet.angle = angle + angleRange * ((float)i / (count - 1) - 0.5f);
			tBullet.angleRate = angleRate;
		}
	}

	public void ShootPlacedNWay(Vector2 pos, float angle, float angleRange, float speed, int count, int moveTime, int stopTime, GameObject bullet)
	{
		for (int i = 0; i < count; i++) {
			GameObject tmp = Instantiate (bullet) as GameObject;
			print (bullet);
			PlacedBullet tBullet = tmp.GetComponent<PlacedBullet> ();
			tmp.transform.position = pos;
			tBullet.angle = angle + angleRange * ((float)i / (count - 1) - 0.5f);
			print (tBullet);
			tBullet.speed = speed;
			tBullet.MoveTime = moveTime;
			tBullet.StopTime = stopTime;
		}		
	}
}
