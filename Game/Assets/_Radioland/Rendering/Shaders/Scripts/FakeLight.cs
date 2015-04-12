using UnityEngine;
using System.Collections;

public class FakeLight : MonoBehaviour {

	public GameObject lightSource;
	public Color lit;
	public Color shade;

	private Color m_lit;
	private Color m_shade;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material.SetColor("_Lit", lit);
		m_lit = lit;
		GetComponent<Renderer>().material.SetColor("_Shade", shade);
		m_shade = shade;
	}
	
	// Update is called once per frame
	void Update () {
		if (m_lit != lit) {
			GetComponent<Renderer>().material.SetColor("_Lit", lit);
			m_lit = lit;
		}
		if (m_shade != shade) {
			GetComponent<Renderer>().material.SetColor("_Shade", shade);
			m_shade = shade;
		}
		GetComponent<Renderer>().material.SetVector("_LightSource", lightSource.transform.position);
	}
}
