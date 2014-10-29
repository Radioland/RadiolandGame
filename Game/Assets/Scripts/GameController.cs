using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject player;
    [Tooltip("Kill the player when they fall below this value.")]
    [SerializeField] private float fallbackKillY = -100.0f;
    [SerializeField] private Vector3 fallbackRespawnPosition = Vector3.zero;

    private GameObject latestCheckpoint;

    void Awake() {
        player = GameObject.FindWithTag("Player");
    }

    void Start() {

    }

    void Update() {
        if (player.transform.position.y < fallbackKillY) {
            KillPlayer();
        }
    }

    public void SetLatestCheckpoint(GameObject checkpoint) {
        latestCheckpoint = checkpoint;
    }

    public void KillPlayer() {
        // TODO: effects, penalty, respawn animation, etc.

        if (latestCheckpoint) {
            Region latestCheckpointRegion = latestCheckpoint.GetComponent<Region>();
            player.transform.position = latestCheckpointRegion.GetCenter();
        } else {
            player.transform.position = fallbackRespawnPosition;
        }
    }
}
