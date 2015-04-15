using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpExtras : MonoBehaviour
{
    #region Editor-specified values.
    [Header("Low Gravity (Floating)")]
    [SerializeField] private float timeToMinGravity = 1f;
    [SerializeField] private AnimationCurve lowGravityStrength = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private float minGravity = 3f;
    [SerializeField] private float newSmoothDampTimes = 1f;
    [SerializeField] private bool lowGravityEnabled;
    [Header("Effects")]
    [SerializeField] private List<TrailRenderer> trails;
    [SerializeField] private Color doubleJumpColor;
    [SerializeField] private Color lowGravityColor;
    [SerializeField] private EffectManager jumpEffects;
    [SerializeField] private EffectManager doubleJumpEffects;
    #endregion Editor-specified values.

    #region Mechanics.
    private CharacterMovement characterMovement;
    private float initialGravity;
    private float initialGroundSmoothDampTime;
    private float initialAirSmoothDampTime;
    private float timeHeld;
    private float lastTimeLanded;
    #endregion Mechanics.

    #region Animation.
    private Animator animator;
    private int longJumpHash;
    private int highJumpHash;
    private int doubleJumpHash;
    #endregion Animation.

    #region Trail color and fading.
    private float trailDoubleJumpStrength;
    private float trailLowGravityStrength;
    private HSBColor doubleJumpColorHSB;
    private HSBColor lowGravityColorHSB;
    private const float trailDoubleJumpFadeInTime = 0.2f;
    private const float trailDoubleJumpFadeOutTime = 0.2f;
    private const float trailLowGravityFadeInTime = 0.5f;
    private const float trailLowGravityFadeOutTime = 0.1f;
    private float trailDoubleJumpSmoothing = 0f;
    private float trailLowGravitySmoothing = 0f;
    private const float trailDeactivateTime = 1.5f;
    private bool doubleJumpIncreasing;
    #endregion Trail color and fading.

    private void Awake() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();

        timeHeld = 0f;
        lastTimeLanded = -1000f;

        longJumpHash = Animator.StringToHash("LongJump");
        highJumpHash = Animator.StringToHash("HighJump");
        doubleJumpHash = Animator.StringToHash("DoubleJump");

        trailDoubleJumpStrength = 0f;
        trailLowGravityStrength = 0f;
        doubleJumpColorHSB = new HSBColor(doubleJumpColor);
        lowGravityColorHSB = new HSBColor(lowGravityColor);
    }

    private void Start() {
        initialGravity = characterMovement.GetInitialGravity();
        initialGroundSmoothDampTime = characterMovement.GetInitialGroundSmoothDampTime();
        initialAirSmoothDampTime = characterMovement.GetInitialAirSmoothDampTime();

        Messenger.AddListener<float>("Grounded", OnGrounded);
        Messenger.AddListener("JumpStarted", OnJumpStarted);
        Messenger.AddListener("DoubleJumpStarted", OnDoubleJumpStarted);
        Messenger.AddListener<string>("FinishedCollecting", OnFinishedCollecting);
    }

    private void Update() {
        bool holdingJump = ((characterMovement.jumping || characterMovement.falling)
                             && Input.GetButton("Jump"));
        if (holdingJump && lowGravityEnabled) {
            timeHeld += Time.deltaTime;
        } else {
            timeHeld -= Time.deltaTime;
        }
        timeHeld = Mathf.Clamp(timeHeld, 0f, timeToMinGravity);

        float t = lowGravityStrength.Evaluate(timeHeld / timeToMinGravity); // range [0,1]
        if (t >= 1) { animator.SetBool(longJumpHash, true); }
        if (lowGravityEnabled) {
            characterMovement.SetGravity(Mathf.Lerp(initialGravity, minGravity, t));
            characterMovement.SetGroundSmoothDampTime(Mathf.Lerp(initialGroundSmoothDampTime, newSmoothDampTimes, t));
            characterMovement.SetAirSmoothDampTime(Mathf.Lerp(initialAirSmoothDampTime, newSmoothDampTimes, t));
        }

        // Tint and fade in trails.
        trailDoubleJumpStrength = doubleJumpIncreasing ?
                Mathf.SmoothDamp(trailDoubleJumpStrength, 1f, ref trailDoubleJumpSmoothing, trailDoubleJumpFadeInTime) :
                Mathf.SmoothDamp(trailDoubleJumpStrength, 0f, ref trailDoubleJumpSmoothing, trailDoubleJumpFadeOutTime);
        if (trailDoubleJumpStrength >= 0.99f) { doubleJumpIncreasing = false; }
        if (doubleJumpIncreasing) {
            trailLowGravityStrength =
                    Mathf.SmoothDamp(trailLowGravityStrength, 0, ref trailLowGravitySmoothing, trailLowGravityFadeOutTime);
        } else {
            trailLowGravityStrength = holdingJump ?
                    Mathf.SmoothDamp(trailLowGravityStrength, t, ref trailLowGravitySmoothing, trailLowGravityFadeInTime) :
                    Mathf.SmoothDamp(trailLowGravityStrength, t, ref trailLowGravitySmoothing, trailLowGravityFadeOutTime);
        }

        float totalStrength = trailDoubleJumpStrength + trailLowGravityStrength;
        float fractionLowGravity = trailLowGravityStrength / totalStrength;

        HSBColor lerpedColor = HSBColor.Lerp(doubleJumpColorHSB, lowGravityColorHSB, fractionLowGravity);
        Color tintColor = lerpedColor.ToColor();
        tintColor.a = totalStrength;
        foreach (TrailRenderer trail in trails) {
            if (characterMovement.grounded && Time.time - lastTimeLanded > trailDeactivateTime) {
                trail.enabled = false;
                trail.gameObject.SetActive(false);
            } else {
                trail.enabled = true;
                trail.gameObject.SetActive(true);
            }
            trail.material.SetColor("_TintColor", tintColor);
        }
    }

    private void OnGrounded(float verticalSpeed) {
        lastTimeLanded = Time.time;
        timeHeld = 0f;
        animator.SetBool(longJumpHash, false);
        animator.SetBool(highJumpHash, false);
        doubleJumpIncreasing = false;
    }

    private void OnJumpStarted() {
        if (jumpEffects) {
            jumpEffects.StartEvent();
        }
    }

    private void OnDoubleJumpStarted() {
        animator.SetBool(highJumpHash, true);
        animator.SetTrigger(doubleJumpHash);
        doubleJumpIncreasing = true;

        if (doubleJumpEffects) {
            doubleJumpEffects.StartEvent();
        }
    }

    private void OnFinishedCollecting(string type) {
        if (type == "babybirb") {
            lowGravityEnabled = true;
        }
    }
}
