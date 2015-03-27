using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpExtras : MonoBehaviour
{
    [SerializeField] private float timeToMinGravity = 1f;
    [SerializeField] private float minGravity = 3f;
    [SerializeField] private float newSmoothDampTimes = 1f;
    [SerializeField] private List<TrailRenderer> trails;

    private CharacterMovement characterMovement;
    private float initialGravity;
    private float initialGroundSmoothDampTime;
    private float initialAirSmoothDampTime;
    private float timeHeld;
    private float lastTimeLanded;

    private Animator animator;
    private int longJumpHash;
    private int highJumpHash;

    private float trailTintAlpha;
    private const float trailFadeInTime = 2f;
    private const float trailFadeOutTime = 0.2f;
    private float trailSmoothing = 0f;
    private const float trailDeactivateTime = 1.5f;

    private void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();

        timeHeld = 0f;
        lastTimeLanded = -1000f;

        longJumpHash = Animator.StringToHash("LongJump");
        highJumpHash = Animator.StringToHash("HighJump");

        trailTintAlpha = 0f;

        Messenger.AddListener<float>("Grounded", OnGrounded);
        Messenger.AddListener("JumpStarted", OnJumpStarted);
        Messenger.AddListener("DoubleJumpStarted", OnDoubleJumpStarted);
    }

    private void Start() {
        initialGravity = characterMovement.GetInitialGravity();
        initialGroundSmoothDampTime = characterMovement.GetInitialGroundSmoothDampTime();
        initialAirSmoothDampTime = characterMovement.GetInitialAirSmoothDampTime();
    }

    private void Update() {
        bool holdingJump = ((characterMovement.jumping || characterMovement.falling)
                             && Input.GetButton("Jump"));
        if (holdingJump) {
            timeHeld += Time.deltaTime;
        } else {
            timeHeld -= Time.deltaTime;
        }
        timeHeld = Mathf.Clamp(timeHeld, 0f, timeToMinGravity);

        float t = timeHeld / timeToMinGravity;
        if (t >= 1) { animator.SetBool(longJumpHash, true); }
        characterMovement.SetGravity(Mathf.Lerp(initialGravity, minGravity, t));
        characterMovement.SetGroundSmoothDampTime(Mathf.Lerp(initialGroundSmoothDampTime, newSmoothDampTimes, t));
        characterMovement.SetAirSmoothDampTime(Mathf.Lerp(initialAirSmoothDampTime, newSmoothDampTimes, t));

        // Tint trails.
        trailTintAlpha = holdingJump ? Mathf.SmoothDamp(trailTintAlpha, t, ref trailSmoothing, trailFadeInTime) :
                                       Mathf.SmoothDamp(trailTintAlpha, t, ref trailSmoothing, trailFadeOutTime);
        Color tintColor = new Color(1f, 1f, 1f, trailTintAlpha);
        foreach (TrailRenderer trail in trails) {
            if (characterMovement.grounded && Time.time - lastTimeLanded > trailDeactivateTime) {
                trail.enabled = false;
            } else {
                trail.enabled = true;
            }
            trail.material.SetColor("_TintColor", tintColor);
        }
    }

    private void OnGrounded(float verticalSpeed) {
        lastTimeLanded = Time.time;
        timeHeld = 0f;
        animator.SetBool(longJumpHash, false);
        animator.SetBool(highJumpHash, false);
    }

    private void OnJumpStarted() {
        // TODO: jump effects? sounds, particles?
    }

    private void OnDoubleJumpStarted() {
        animator.SetBool(highJumpHash, true);
    }
}
