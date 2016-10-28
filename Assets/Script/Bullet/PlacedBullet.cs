using UnityEngine;
using System.Collections;

public class PlacedBullet : MonoBehaviour {
	public float InitialSpeed;
	public int MoveTime;
	public int StopTime;
	public int Timer;
	public float angle = 0;
	public float angleRate = 0;
	public float speed = 0;
	public float speedRate = 0;

	void Update () {
		if (Timer == MoveTime)
			speed = 0;

		if (Timer == MoveTime + StopTime)
			speed = InitialSpeed;

		Timer++;

		float rad = angle * Mathf.PI * 2;

		transform.position = new Vector2 (transform.position.x + (speed * Mathf.Cos (rad) * Time.deltaTime), transform.position.y + speed * Mathf.Sin (rad) * Time.deltaTime);

		angle += angleRate;
		speed += speedRate;
	}
}
