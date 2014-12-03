using UnityEngine;
using System.Collections;

public class PlayerLevelManager : MonoBehaviour
{
    void Awake() {
        PlayerPrefs.SetInt("level", Application.loadedLevel);
    }

    void Update() {

    }
}
