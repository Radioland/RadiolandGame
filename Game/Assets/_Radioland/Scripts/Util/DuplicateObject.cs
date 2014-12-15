using UnityEngine;
using System.Collections;

public class DuplicateObject : MonoBehaviour {

	public bool flipPosX;
	public bool flipPosY;
	public bool flipPosZ;

	public bool flipRotX;
	public bool flipRotY;
	public bool flipRotZ;

	// Use this for initialization
	void Start () {
		Vector3 pos = transform.localPosition;
		if (flipPosX)
			pos.x *= -1;
		if (flipPosY)
			pos.y *= -1;
		if (flipPosZ)
			pos.z *= -1;

		Vector3 rot = transform.localRotation.eulerAngles;
		if (flipRotX)
			rot.x += 180;
		if (flipRotY)
			rot.y += 180;
		if (flipRotZ)
			rot.z += 180;

		GameObject duplicate = (GameObject)Instantiate(gameObject);
		duplicate.transform.parent = transform.parent;
		duplicate.transform.localPosition = pos;
		duplicate.transform.localRotation = Quaternion.Euler(rot);
		duplicate.transform.localScale = transform.localScale;
		Destroy(duplicate.GetComponent<DuplicateObject>());
	}

}
