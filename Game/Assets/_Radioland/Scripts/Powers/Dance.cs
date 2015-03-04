﻿using UnityEngine;
using System.Collections;

public class Dance : MonoBehaviour
{
    [HideInInspector] public bool dancing;
    [SerializeField] private float idleDanceStartTime = 4.0f;
    [SerializeField] private float minDanceTime = 0.5f;
    [SerializeField] private float minInputStopTime = 0.5f;
    [SerializeField] private float inputPersistTime = 0.1f;
    [SerializeField] private EffectManager danceEffects;

    private CharacterMovement characterMovement;
    private Animator animator;
    private int danceBoolHash;
    private int danceStateHash;
    private bool stopping;
    private float lastDanceTime;
    private float lastInputTime;
    private float lastInputStartedTime;
    private bool stillNoInput;
    private bool ableToDance;

    private void Awake() {
        dancing = false;

        characterMovement = gameObject.GetComponent<CharacterMovement>();

        // Fetch animator properties.
        animator = gameObject.GetComponentInChildren<Animator>();
        if (!animator) {
            Debug.LogWarning("No animator found on " + transform.GetPath());
        }
        danceBoolHash = Animator.StringToHash("Dancing");
        danceStateHash = Animator.StringToHash("Base Layer.Dance");

        ableToDance = false;
        stopping = false;
        lastDanceTime = -1000.0f;
        lastInputTime = -1000.0f;
        lastInputStartedTime = -1000.0f;
        stillNoInput = true;
    }

    private void Start() {

    }

    private void Update() {
        if (ableToDance && stillNoInput && Time.time - lastInputStartedTime > idleDanceStartTime) {
            StartDancing();
        }

        if (!ableToDance) {
            stopping = true;
        }

        bool inDanceState = animator.GetCurrentAnimatorStateInfo(0).nameHash == danceStateHash;

        // Wait at least minDanceTime before stopping the animation and restoring control.
        if (Time.time - lastDanceTime > minDanceTime && stopping) {
            if (inDanceState) {
                animator.SetBool(danceBoolHash, false);
            }

            if (!inDanceState) {
                StopDancing();
            }
        }

        if (dancing) {
            characterMovement.SetControllable(false);
        }
    }

    public void SetAbleToDance(bool newAbleToDance) {
        ableToDance = newAbleToDance;
    }

    // Called via SendMessage in CharacterMovement.
    private void InputReceived() {
        lastInputTime = Time.time;

        if (dancing && !stillNoInput) {
            if (Time.time - lastInputStartedTime > minInputStopTime) {
                stopping = true;
            }
        } else {
            lastInputStartedTime = Time.time;
        }
        stillNoInput = false;
    }

    // Called via SendMessage in CharacterMovement.
    private void NoMovementInput() {
        if (Time.time - lastInputTime > inputPersistTime) {
            stillNoInput = true;
        }
    }

    // Called via SendMessage in CharacterMovement.
    private void JumpTriggered() {
        // Jump immediately cancels dancing.
        if (dancing) {
            StopDancing();
        }
    }

    private void StartDancing() {
        dancing = true;
        stopping = false;
        lastDanceTime = Time.time;

        animator.SetBool(danceBoolHash, true);

        if (danceEffects) {
            danceEffects.StartEvent();
        }
    }

    private void StopDancing() {
        animator.SetBool(danceBoolHash, false);
        characterMovement.SetControllable(true);
        stopping = false;
        dancing = false;

        if (danceEffects) {
            danceEffects.StopEvent();
        }
    }
}
