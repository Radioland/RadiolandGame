using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Text uiText;

    [SerializeField] private List<TextAsset> messages;

    private void Awake() {

    }

    private void Start() {

    }

    private void Update() {
        // if near the player and press dialogue input, appear or cycle
        // if the player moves away, reset to the start
    }
}
