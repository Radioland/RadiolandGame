using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject player;
    [Tooltip("Kill the player when they fall below this value.")]
    [SerializeField] private float fallbackKillY = -100.0f;
    public bool masterMute = false;

    private Vector3 fallbackRespawnPosition;
    private GameObject latestCheckpoint;

    void Awake() {
        player = GameObject.FindWithTag("Player");

        fallbackRespawnPosition = player.transform.position;
    }

    void Start() {

    }

    void Update() {
        if (player.transform.position.y < fallbackKillY) {
            KillPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit();
        }

        if (Input.GetKeyDown(KeyCode.M)) { masterMute = !masterMute; }
        AudioListener.volume = masterMute ? 0.0f : 1.0f;
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

    public void Exit() {
        Application.Quit();
    }
}
