using UnityEngine;
using System.Collections;

public class UICosmeticIcon : MonoBehaviour
{
    [SerializeField] private string cosmeticName;
    public bool isUnlocked;
    public Material unlockedMaterial;
    private bool materialSet;
    [SerializeField] private float rotationDegreesPerSecond = 30f;

    private Quaternion initialRotation;
    private bool rotating;

    private void Awake() {
        initialRotation = transform.rotation;
        materialSet = false;
        rotating = false;

        CosmeticsManager.Register(cosmeticName);

        if (CosmeticsManager.IsNotLocked(cosmeticName)) {
            GetComponent<Renderer>().material = unlockedMaterial;
            materialSet = true;
        }
    }

    private void Update() {
        transform.Rotate(Vector3.up, rotationDegreesPerSecond * (rotating ? 1 : 0) * Time.deltaTime);

//        if (isUnlocked && !materialSet) {
//            GetComponent<Renderer>().material = unlockedMaterial;
//            materialSet = true;
//        }
    }

    public void startRotate() {
        rotating = true;
    }

    public void stopRotate() {
        transform.rotation = initialRotation;
        rotating = false;
    }
}
