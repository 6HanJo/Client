using UnityEngine;
using System.Collections;

public class MoveBackground : MonoBehaviour {

	public float scrollSpeed = 0.01f;
	public int backGroundType;

	public Texture back1;
	public Texture back2;
	public Texture back3;

	Material myMaterial;

	void Start () {
		myMaterial = GetComponent<Renderer> ().material;
		if(backGroundType == 1)
			myMaterial.mainTexture = back1;
		else if(backGroundType == 2)
			myMaterial.mainTexture = back2;
		else if(backGroundType == 3)
			myMaterial.mainTexture = back3;
		myMaterial.mainTextureOffset = new Vector2 (0, 0);
	}

	void Update () { 
		float newOffsetY = myMaterial.mainTextureOffset.y + scrollSpeed * Time.deltaTime;

		myMaterial.mainTextureOffset = new Vector2 (0, newOffsetY);
	}
}
