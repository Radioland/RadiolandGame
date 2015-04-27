using UnityEngine;
using System.Collections;

public class RandomRotation : MonoBehaviour {

	[Tooltip("Euler angle degrees range, negative to positive.")]
	[SerializeField] private Vector3 angularRange;

	void Start () {
	
	}
	
	void Update () {
		float x = Random.Range(-angularRange.x, angularRange.x);
		float y = Random.Range(-angularRange.y, angularRange.y);
		float z = Random.Range(-angularRange.z, angularRange.z);
		transform.Rotate(new Vector3(x,y,z));
	}
}
