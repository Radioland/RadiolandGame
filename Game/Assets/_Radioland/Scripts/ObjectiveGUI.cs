using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveGUI : MonoBehaviour {
    private int targetsFound;
    private int totalTargets;

    private Text text;

    private void Start() {
        targetsFound = 0;
        totalTargets = 5;
        text = GetComponent<Text>();
    }

    private void Update() {
        text.text = targetsFound.ToString() + "/" + totalTargets.ToString();
    }

    public void SetTotalTargets(int targets) {
        totalTargets = targets;
    }

    public void IncrementTargets() {
        targetsFound += 1;
    }
}
