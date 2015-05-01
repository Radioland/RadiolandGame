using UnityEngine;
using System.Collections;

public class LevelSelectController : MonoBehaviour
{
    private LevelSelectEntry[] levelSelectEntries;

    private void Awake() {
        levelSelectEntries = gameObject.GetComponentsInChildren<LevelSelectEntry>();
    }

    private void Start() {

    }

    private void Update() {

    }
}
