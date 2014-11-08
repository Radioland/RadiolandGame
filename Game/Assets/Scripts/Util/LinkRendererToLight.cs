using UnityEngine;
using System.Collections;

public class LinkRendererToLight : MonoBehaviour {
	
	public Light theLight;
	public string shaderColorName = "_Color";

	void Awake () {
		if (theLight == null) {
			theLight = gameObject.GetComponentInParent<Light>();
		}
		if (!renderer.material.HasProperty(shaderColorName)) {
			Debug.LogWarning(renderer.GetPath() + " has no material color named " + shaderColorName);
		}
	}

	// Update is called once per frame
	void Update () {
		if (renderer.material.HasProperty(shaderColorName)) {
			if (renderer.material.GetColor(shaderColorName) != theLight.color) {
				renderer.material.SetColor(shaderColorName, theLight.color);
			}
		}
	}
}
