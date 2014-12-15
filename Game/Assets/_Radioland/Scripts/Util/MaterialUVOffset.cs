using UnityEngine;
using System.Collections;

public class MaterialUVOffset : MonoBehaviour {

	public Vector2 offset;

	public bool random;

	public string shaderColorName = "_MainTex";
	// Use this for initialization
	void Start () {

		if (random) {
			offset.x = Random.Range(0f, 1f);
			offset.y = Random.Range(0f, 1f);
		}

		if (!renderer.material.HasProperty(shaderColorName)) {
			Debug.LogWarning(renderer.GetPath() + " has no material color named " + shaderColorName);
		}
		else {
			renderer.material.SetTextureOffset(shaderColorName, offset);
		}
	}

}
