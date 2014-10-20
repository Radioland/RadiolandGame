using UnityEngine;
using System.Collections;

public class ObjectPickup : MonoBehaviour {
    GameObject objectiveGUI;

    void Start () {
        objectiveGUI = GameObject.Find ("ObjectiveUI");
    }

    void OnTriggerEnter (Collider c) {
        if (c.gameObject.tag == "pickupable") {
            Destroy(c.gameObject);
            objectiveGUI.GetComponent<ObjectiveGUI>().IncrementTargets();
        }
    }
    
    void Update () {

    }
    
}
