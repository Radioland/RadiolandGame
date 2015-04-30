using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoScrollbar : MonoBehaviour {

	public Scrollbar m_scrollbar;

	public float time = 1f;
	private float m_time;

	// Use this for initialization
	void Start () {
		m_time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		m_scrollbar.value = Mathf.Lerp(1, 0, Mathf.Repeat(m_time + Time.time / time, 1));
	}
}
