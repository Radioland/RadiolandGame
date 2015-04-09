using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollectiblesManager : MonoBehaviour
{
    // Dictionary keys are the collectible types.
    private Dictionary<string, List<Collectible>> collectibleGroups;
    private Dictionary<string, int> collectedCounts;
    private Dictionary<string, int> collectibleTotals;

    private void Awake() {
        collectibleGroups = new Dictionary<string, List<Collectible>>();
        collectedCounts = new Dictionary<string, int>();
        collectibleTotals = new Dictionary<string, int>();

        // Find collectibleGroups and setup dictionary.
        GameObject[] collectibleObjects = GameObject.FindGameObjectsWithTag("collectible");
        foreach (GameObject collectibleObject in collectibleObjects) {
            Collectible collectibleScript = collectibleObject.GetComponent<Collectible>();
            if (!collectibleScript) { continue; }

            string type = collectibleScript.type;
            if (!collectibleGroups.ContainsKey(type)) {
                collectibleGroups.Add(type, new List<Collectible>());
            }

            collectibleGroups[type].Add(collectibleScript);
            collectedCounts[type] = 0;
            collectibleTotals[type] = collectibleGroups[type].Count;
        }

        Debug.Log("Types of collectibleGroups found: " + collectibleGroups.Count);

        Messenger.AddListener<string, bool>("CollectObject", OnCollectObject);
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnCollectObject(string type, bool playAnim) {
        Debug.Log("Collected " + type);
        collectedCounts[type]++;
    }
}
