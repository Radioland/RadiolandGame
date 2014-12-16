using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveGUI : MonoBehaviour {
    int targetsFound;
    int totalTargets;
    
    Text text;

    void Start () {
        targetsFound = 0;
        totalTargets = 5;
        text = GetComponent<Text>();
    }

    void Update () {
        text.text = targetsFound.ToString() + "/" + totalTargets.ToString();
    }
    
    public void SetTotalTargets(int targets) {
        totalTargets = targets;
    }
    
    public void IncrementTargets() {
        targetsFound += 1;
    }
}
