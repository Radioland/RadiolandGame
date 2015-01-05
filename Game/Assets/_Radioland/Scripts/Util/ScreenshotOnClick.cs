using UnityEngine;
using System.Collections;

public class ScreenshotOnClick : MonoBehaviour {

    public int superSize = 1;

    private bool takeHiResShot = false;

    private void LateUpdate() {
        takeHiResShot |= Input.GetKeyDown("g");
        if (takeHiResShot)
        {
            Application.CaptureScreenshot("screenshots/screenshot"+System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")+".png", superSize);
            Debug.Log("Took screenshot");
            takeHiResShot = false;
        }
    }
}
