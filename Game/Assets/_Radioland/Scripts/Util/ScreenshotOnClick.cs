using UnityEngine;
using System.Collections;

public class ScreenshotOnClick : MonoBehaviour {

	public int superSize = 1;
	
	private bool takeHiResShot = false;
	
	void LateUpdate() {
		takeHiResShot |= Input.GetKeyDown("k");
		if (takeHiResShot) 
		{
			Application.CaptureScreenshot("Screenshot.png", superSize);
			Debug.Log("Took screenshot");
			takeHiResShot = false;
		}
	}
}
