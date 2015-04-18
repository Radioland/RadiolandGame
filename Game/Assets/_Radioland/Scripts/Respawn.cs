using UnityEngine;
using System.Collections;

// Handles respawn behavior, making use of Effects as well.
// At time of writing, this script is fragile (sorry!), edit with caution.
// Due for a refactor when time permits.

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject respawnPrefab;
    [SerializeField] private EffectManager respawnEffects;
    [SerializeField] private float respawnTime = 8.0f;

    [SerializeField] private TurnScriptsOnOffEffect scriptsDisableEffect;
    [SerializeField] private SetActiveEffect playerDisableEffect;
    [SerializeField] private SetTransformEffect playerTransformEffect;
    [SerializeField] private PositionCameraEffect positionCameraEffect;

    private GameObject player;
    private CharacterMovement characterMovement;
    private CameraControl cameraControl;
    private bool respawning;

    private void Awake() {
        respawning = false;

        player = GameObject.FindWithTag("Player");
        characterMovement = player.GetComponent<CharacterMovement>();
        cameraControl = player.GetComponent<CameraControl>();

        GameObject hairObject = GameObject.FindWithTag("hair");
        Hair hairScript = hairObject.GetComponent<Hair>();
        HairEffect hairEffect = gameObject.GetComponentInChildren<HairEffect>();
        hairEffect.hair = hairScript;

        positionCameraEffect.cameraTransform = Camera.main.transform;
        positionCameraEffect.lookAtTarget = player.transform;

        scriptsDisableEffect.scripts.Add(characterMovement);
        scriptsDisableEffect.scripts.Add(player.GetComponent<CameraControl>());
        scriptsDisableEffect.scripts.Add(player.GetComponentInChildren<RadioControl>());

        playerDisableEffect.objectToSetActive = GameObject.FindWithTag("playermodel");
    }

    private void Start() {

    }

    private void Update() {

    }

    public void RespawnPlayer(Vector3 respawnPosition) {
        if (respawning) { return; }
        respawning = true;

        playerTransformEffect.objectTransform = player.transform;
        playerTransformEffect.position = respawnPosition;
        playerTransformEffect.rotationEulerAngles = Vector3.zero;
        playerTransformEffect.scale = player.transform.localScale;

        respawnEffects.StartEvent();

        Invoke("FinishRespawn", respawnTime);
    }

    private void FinishRespawn() {
        respawning = false;

        characterMovement.ResetState();
        characterMovement.Stop();

        cameraControl.HandleRespawn();
    }
}
