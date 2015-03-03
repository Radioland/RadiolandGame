using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject player;
    [Tooltip("Kill the player when they fall below this value.")]
    [SerializeField] private float fallbackKillY = -100.0f;
    public bool masterMute = false;

    [SerializeField] private GameObject pauseBackground;
    [SerializeField] private Button resumeButton;
    private bool m_paused = false;
    public bool paused { get { return m_paused; } }

    private Vector3 fallbackRespawnPosition;
    private GameObject latestCheckpoint;
    private Respawn respawnScript;

    private void Awake() {
        player = GameObject.FindWithTag("Player");

        fallbackRespawnPosition = player.transform.position;

        respawnScript = gameObject.GetComponentInChildren<Respawn>();
    }

    private void Start() {

    }

    private void Update() {
        if (player.transform.position.y < fallbackKillY) {
            KillPlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Exit();
        }

        if (Input.GetKeyDown(KeyCode.M)) { masterMute = !masterMute; }
        AudioListener.volume = masterMute ? 0.0f : 1.0f;

        HandlePause();

        HandleCheats();
    }

    private void HandlePause() {
        if (Input.GetButtonDown("Pause") && !paused) {
            TogglePause();
        }
    }

    public void TogglePause() {
        m_paused = !paused;

        Time.timeScale = paused ? 0.0f : 1.0f;
        AudioListener.pause = paused;
        if (pauseBackground) {
            pauseBackground.SetActive(paused);

        }
        if (resumeButton && paused) {
            EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            eventSystem.SetSelectedGameObject(resumeButton.gameObject, new BaseEventData(eventSystem));
        }

        CharacterMovement characterMovement = player.GetComponent<CharacterMovement>();
        characterMovement.SetControllable(!paused);
    }

    private void HandleCheats() {
        if (Input.GetKeyDown(KeyCode.Equals)) {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
        if (Input.GetKeyDown(KeyCode.Minus)) {
            Application.LoadLevel(Application.loadedLevel - 1);
        }
    }

    public void SetLatestCheckpoint(GameObject checkpoint) {
        latestCheckpoint = checkpoint;
    }

    public void KillPlayer() {
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
