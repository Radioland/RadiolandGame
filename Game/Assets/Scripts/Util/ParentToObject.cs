using UnityEngine;
using System.Collections;

public class ParentToObject : MonoBehaviour {

	public GameObject Parent;

	// Use this for initialization
	void Start () {
		transform.parent = Parent.transform;
	}
}
