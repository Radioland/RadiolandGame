using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICosmeticIcon : MonoBehaviour
{
    [Header("General UI Settings")]
    [SerializeField] private float rotationDegreesPerSecond = 30f;
    [SerializeField] private Button button;

    [Header("Cosmetic Settings")]
    [SerializeField] private string cosmeticName;
    [SerializeField] private Material unlockedMaterial;

    private Quaternion initialRotation;
    private bool rotating;

    private void Awake() {
        initialRotation = transform.rotation;
        rotating = false;

        CosmeticsManager.Register(cosmeticName);

        if (CosmeticsManager.IsNotLocked(cosmeticName)) {
            GetComponent<Renderer>().material = unlockedMaterial;
            if (button) { button.interactable = true; }
        } else {
            if (button) { button.interactable = false; }
        }
    }

    private void Update() {
        transform.Rotate(Vector3.up, rotationDegreesPerSecond * (rotating ? 1 : 0) * Time.deltaTime);
    }

    public void startRotate() {
        rotating = true;
    }

    public void stopRotate() {
        transform.rotation = initialRotation;
        rotating = false;
    }
}
