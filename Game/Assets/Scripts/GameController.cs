using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject player;

    private GameObject latestCheckpoint;

    void Awake() {
        player = GameObject.FindWithTag("Player");
    }

    void Start() {

    }

    void Update() {

    }

    public void SetLatestCheckpoint(GameObject checkpoint) {
        latestCheckpoint = checkpoint;
    }

    public void KillPlayer() {
        // TODO: effects, penalty, respawn animation, etc.

        Region latestCheckpointRegion = latestCheckpoint.GetComponent<Region>();
        player.transform.position = latestCheckpointRegion.GetCenter();
    }
}
