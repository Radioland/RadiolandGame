using UnityEngine;
using System.Collections;

public class PlayerLevelManager : MonoBehaviour
{
    private void Awake() {
        PlayerPrefs.SetInt("level", Application.loadedLevel);
    }

    private void Update() {

    }
}
