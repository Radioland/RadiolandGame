using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour
{
    private GameObject[] jumpControlledPlatforms;

    private void Awake() {
        jumpControlledPlatforms = GameObject.FindGameObjectsWithTag("jumpplatform");
    }

    private void Update() {

    }

    public void ControlPlatforms() {
        foreach (GameObject jumpControlledPlatform in jumpControlledPlatforms) {
            jumpControlledPlatform.SendMessage("PerformAction");
        }
    }
}
