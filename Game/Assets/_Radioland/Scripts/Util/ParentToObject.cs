using UnityEngine;
using System.Collections;

public class ParentToObject : MonoBehaviour {

	public GameObject Parent;

	public bool hasOffset;
	public Vector3 offset;

	// Use this for initialization
	void Start () {
		transform.parent = Parent.transform;
		if (hasOffset) {
			transform.position = transform.parent.transform.position + offset;
		}
	}
}
