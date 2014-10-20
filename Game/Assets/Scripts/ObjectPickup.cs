using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {
    GameObject objectiveGUI;
    // Use this for initialization
    void Start () {
        objectiveGUI = GameObject.Find ("ObjectiveUI");
    }

    void OnTriggerEnter (Collider c) {
        if (c.gameObject.tag == "pickupable") {
            Destroy(c.gameObject);
            objectiveGUI.GetComponent<ObjectiveGUI>().IncrementTargets();
        }
    }
    
    // Update is called once per frame
    void Update () {

    }
    
}
