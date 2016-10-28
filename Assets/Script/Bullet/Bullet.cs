using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float angle;
	public float angleRate;
	public float speed;
	public float speedRate;

	void Update () {
		float rad = angle * Mathf.PI * 2;

		transform.position = new Vector2 (transform.position.x + (speed * Mathf.Cos (rad) * Time.deltaTime), transform.position.y + speed * Mathf.Sin (rad) * Time.deltaTime);

		angle += angleRate;
		speed += speedRate;
	}
}
