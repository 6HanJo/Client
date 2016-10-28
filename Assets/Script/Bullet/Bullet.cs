using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float angle = 0;
	public float angleRate = 0;
	public float speed = 0;
	public float speedRate = 0;

	void Update () {
		float rad = angle * Mathf.PI * 2;

		transform.position = new Vector2 (transform.position.x + (speed * Mathf.Cos (rad) * Time.deltaTime), transform.position.y + speed * Mathf.Sin (rad) * Time.deltaTime);

		angle += angleRate;
		speed += speedRate;
	}
}
