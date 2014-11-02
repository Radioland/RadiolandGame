using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
	public GameObject target;
	
	void  Awake ()
	{
		// if no object referenced, grab the main camera
		if (!target)
			target = Camera.main.gameObject; 
	}
	
	void  Update ()
	{
		transform.LookAt (target.transform);
	}
}