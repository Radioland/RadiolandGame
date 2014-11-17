using UnityEngine;
using System.Collections;

public class SubstancePingPong : MonoBehaviour {
	public string floatRangeProperty = "Disorder";
	public float scale = 100;
	public float cycleTime = 10;
	void Update() {
		ProceduralMaterial substance = renderer.sharedMaterial as ProceduralMaterial;
		if (substance) {
			float lerp = Mathf.PingPong(Time.time * 2 / cycleTime, 1) * scale;
			substance.SetProceduralFloat(floatRangeProperty, lerp);
			substance.RebuildTextures();
		}
	}
}