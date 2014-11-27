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
    private Respawn respawnScript;

    void Awake() {
        player = GameObject.FindWithTag("Player");

        fallbackRespawnPosition = player.transform.position;

        respawnScript = gameObject.GetComponentInChildren<Respawn>();
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

        // Cheats
        if (Input.GetKeyDown(KeyCode.Equals)) {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Application.LoadLevel(Application.loadedLevel - 1);
        }

        if (Input.GetKeyDown(KeyCode.M)) { masterMute = !masterMute; }
        AudioListener.volume = masterMute ? 0.0f : 1.0f;
    }

    public void SetLatestCheckpoint(GameObject checkpoint) {
        latestCheckpoint = checkpoint;
    }

    public void KillPlayer() {
        // TODO: effects, penalty, respawn animation, etc.

        Vector3 respawnPosition;
        if (latestCheckpoint) {
            Region latestCheckpointRegion = latestCheckpoint.GetComponent<Region>();
			respawnPosition = latestCheckpointRegion.GetCenter();
        } else {
			respawnPosition = fallbackRespawnPosition;
        }
        respawnScript.RespawnPlayer(respawnPosition);
    }

    public void Exit() {
        Application.Quit();
    }
}
