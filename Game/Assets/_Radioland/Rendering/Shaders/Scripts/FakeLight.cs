using UnityEngine;
using System.Collections;

public class FakeLight : MonoBehaviour {

	public GameObject lightSource;
	public Color lit;
	public Color shade;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.SetColor("_Lit", lit);
		GetComponent<Renderer>().material.SetColor("_Shade", shade);
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().material.SetVector("_LightSource", lightSource.transform.position);
	}
}
