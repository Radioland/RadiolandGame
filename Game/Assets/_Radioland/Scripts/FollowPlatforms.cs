﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class FollowPlatforms : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private float lingerTime = 1.0f;
    [SerializeField] private LayerMask layerMask;

    private CharacterController controller;
    private CharacterMovement characterMovement;
    private Platform latestPlatform;
    private float lastPlatformUnderTime;

    private int pushesThisFrame;
    private const int maxPushesPerFrame = 5;

    private void Awake() {
        controller = gameObject.GetComponent<CharacterController>();
        characterMovement = gameObject.GetComponent<CharacterMovement>();

        if (!controller) {
            Debug.LogWarning(transform.GetPath() + " does not have a CharacterController.");
            this.enabled = false;
        }

        lastPlatformUnderTime = -1000f;
    }

    private void Start() {
        Messenger.AddListener("Jump", OnJump);
        Messenger.AddListener<float>("Grounded", OnGrounded);
    }

    private void Update() {
        pushesThisFrame = 0;

        Platform platform = GetPlatformUnder();
        if (platform) {
            latestPlatform = platform;
            lastPlatformUnderTime = Time.time;
        }

        if (Time.time - lastPlatformUnderTime > lingerTime) {
            latestPlatform = null;
        }

        if (latestPlatform) {
            Push(latestPlatform);
        }
    }

    private void OnJump() {
        if (!characterMovement) { return; }

        if (latestPlatform && latestPlatform.pushPlayerJumping) {
            characterMovement.AddVelocity(latestPlatform.lastVelocity);
            latestPlatform = null;
        }
    }

    private void OnGrounded(float verticalSpeed) {
        SpringPlatform springPlatform = GetSpringPlatformUnder();
        if (springPlatform) {
            springPlatform.ApplyForce(verticalSpeed * characterMovement.mass);
        }
    }

    private Platform GetPlatformUnder() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        Debug.DrawLine(start, start + Vector3.down * distance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, distance, layerMask).OrderBy(h=>h.distance).ToArray();
        return hits.Select(hit => Platform.GetPlatformOnObject(hit.collider.gameObject)).FirstOrDefault(platform => platform);
    }

    private SpringPlatform GetSpringPlatformUnder() {
        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, distance, layerMask).OrderBy(h=>h.distance).ToArray();
        return hits.Select(hit => SpringPlatform.GetSpringPlatformOnObject(hit.collider.gameObject)).FirstOrDefault(platform => platform);
    }

    private void Push(Platform platform) {
        PushWithNormal(platform, Vector3.zero);
    }

    private void PushWithNormal(Platform platform, Vector3 normal) {
        if (Time.timeScale < 0.01f) { return; }
        if (pushesThisFrame > maxPushesPerFrame) { return; }

        if (!platform.pushPlayerJumping && !characterMovement.grounded) { return; }
        if (!platform.pushPlayerGrounded && characterMovement.grounded) { return; }

        Vector3 movement = platform.lastVelocity * Time.deltaTime;
        if (movement.magnitude > 0.01f) {
            pushesThisFrame++;
            // Apply an extra push vertically to prevent falling through platforms.
            movement.y = movement.y * 1.5f + 0.02f;

            movement += normal * movement.magnitude * 2f;

            controller.Move(movement);
        }

    }

    private void OnCollisionEnter(Collision collision) {
        Platform platform = Platform.GetPlatformOnObject(collision.gameObject);
        if (platform) { Push(platform); }
    }

    private void OnCollisionStay(Collision collision) {
        Platform platform = Platform.GetPlatformOnObject(collision.gameObject);
        if (platform) { Push(platform); }
    }

    public void OnControllerColliderHit(ControllerColliderHit hit) {
        Platform platform = Platform.GetPlatformOnObject(hit.collider.gameObject);
        if (platform && platform.pushWithNormal) {
            PushWithNormal(platform, hit.normal);
        }
    }
}
