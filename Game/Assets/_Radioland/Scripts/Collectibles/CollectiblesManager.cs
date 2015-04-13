using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CollectiblesManager : MonoBehaviour
{
    [SerializeField] private Transform layoutPanel;
    [SerializeField] private GameObject collectiblesUI;
    [SerializeField] private GameObject collectibleEntryPrefab;

    // Dictionary keys are the collectible types.
    private Dictionary<string, List<Collectible>> collectibleGroups;
    private Dictionary<string, int> collectedCounts;
    private Dictionary<string, int> collectibleTotals;
    private Dictionary<string, Text> collectibleTexts;

    private void Awake() {
        collectibleGroups = new Dictionary<string, List<Collectible>>();
        collectedCounts = new Dictionary<string, int>();
        collectibleTotals = new Dictionary<string, int>();
        collectibleTexts = new Dictionary<string, Text>();

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

        if (collectibleGroups.Count > 0) {
            collectiblesUI.SetActive(true);
        }

        var sortedGroups = collectibleGroups.Values.OrderByDescending(group => group[0].sortPriority);
        foreach (List<Collectible> collectibles in sortedGroups) {
            string type = collectibles[0].type;

            GameObject collectibleEntry = Instantiate(collectibleEntryPrefab);
            collectibleEntry.transform.SetParent(layoutPanel, worldPositionStays:false);

            Text text = collectibleEntry.GetComponentInChildren<Text>();
            text.text = "0/" + collectibleTotals[type];
            collectibleTexts.Add(type, text);

            collectibleEntry.GetComponentInChildren<Image>().sprite = collectibles[0].image;
        }

        Messenger.AddListener<string, bool>("CollectObject", OnCollectObject);
    }

    private void Start() {

    }

    private void Update() {

    }

    private void OnCollectObject(string type, bool playAnim) {
        collectedCounts[type]++;

        collectibleTexts[type].text = collectedCounts[type] + "/" + collectibleTotals[type];

        if (collectedCounts[type] == collectibleTotals[type]) {
            Messenger.Broadcast("FinishedCollecting", type);
        }
    }
}
