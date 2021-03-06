﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICosmeticIcon : MonoBehaviour
{
    [Header("General UI Settings")]
    [SerializeField] private Transform rotationRoot;
    [SerializeField] private float rotationDegreesPerSecond = 30f;
    [SerializeField] private Button button;
    [SerializeField] private Text equipText;
    [SerializeField] private Image lockedImage;

    [Header("Cosmetic Settings")]
    [SerializeField] private string cosmeticName;
    [SerializeField] private Material unlockedMaterial;

    private Quaternion initialRotation;
    private bool rotating;
    private bool selected;

    private void Reset() {
        rotationRoot = transform;
    }

    private void Awake() {
        initialRotation = transform.rotation;
        rotating = false;

        CosmeticsManager.Register(cosmeticName);

        if (CosmeticsManager.IsNotLocked(cosmeticName)) {
            GetComponent<Renderer>().material = unlockedMaterial;
            if (button) { button.interactable = true; lockedImage.enabled = false; }
        } else {
            if (button) { button.interactable = false; lockedImage.enabled = true; }
        }
    }

    private void Start() {
        Messenger.AddListener<string>("UnlockCosmetic", OnUnlockCosmetic);
    }

    private void Update() {
        if (rotationRoot) {
            rotationRoot.Rotate(Vector3.up, rotationDegreesPerSecond * (rotating ? 1 : 0) * Time.deltaTime);
        }

        if (CosmeticsManager.IsEquipped(cosmeticName)) {
            equipText.enabled = true;
            rotating = true;
        } else {
            equipText.enabled = false;
            if (!selected) {
                StopRotate();
            }
        }
    }

    public void startSelect() {
        rotating = true;
        selected = true;
    }

    public void stopSelect() {
        if (!CosmeticsManager.IsEquipped(cosmeticName)) {
            StopRotate();
        }
        selected = false;
    }

    private void StopRotate() {
        if (rotationRoot) {
            rotationRoot.rotation = initialRotation;
        }
        rotating = false;
    }

    public void ToggleEquip() {
        if (CosmeticsManager.IsEquipped(cosmeticName)) {
            CosmeticsManager.Unequip(cosmeticName);
        } else {
            CosmeticsManager.Equip(cosmeticName);
        }
    }

    private void OnUnlockCosmetic(string unlockedCosmeticName) {
        if (unlockedCosmeticName == cosmeticName) {
            GetComponent<Renderer>().material = unlockedMaterial;
            if (button) { button.interactable = true; lockedImage.enabled = false; }
        }
    }
}
