using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject respawnPrefab;
    [SerializeField] private EffectManager respawnEffects;
    [SerializeField] private float respawnTime = 8.0f;

    private GameObject player;
    private CharacterMovement characterMovement;
    private CameraControl cameraControl;
    private Transform cameraTransform;
    private bool respawning;

    [SerializeField] private TurnScriptOnOffEffect movementDisableEffect;
    [SerializeField] private TurnScriptOnOffEffect cameraDisableEffect;
    [SerializeField] private SetActiveEffect playerDisableEffect;
    [SerializeField] private SetTransformEffect playerTransformEffect;
    [SerializeField] private PositionCameraEffect positionCameraEffect;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        characterMovement = player.GetComponent<CharacterMovement>();
        cameraControl = player.GetComponent<CameraControl>();

        cameraTransform = Camera.main.transform;

        respawning = false;
    }

    void Start() {

    }

    void Update() {

    }

    public void RespawnPlayer(Vector3 respawnPosition) {
        if (respawning) {
            return;
        }

        movementDisableEffect.script = characterMovement;
        cameraDisableEffect.script = cameraControl;
        playerDisableEffect.objectToSetActive = player;

        playerTransformEffect.objectTransform = player.transform;
        playerTransformEffect.position = respawnPosition;
        playerTransformEffect.rotationEulerAngles = Vector3.zero;
        playerTransformEffect.scale = player.transform.localScale;

        positionCameraEffect.cameraTransform = cameraTransform;
        positionCameraEffect.lookAtTarget = player.transform;

        respawnEffects.StartEvent();

        respawning = true;
        Invoke("FinishRespawn", respawnTime);
    }

    void FinishRespawn() {
        respawning = false;
    }
}
