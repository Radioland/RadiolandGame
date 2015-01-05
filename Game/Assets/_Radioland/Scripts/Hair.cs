using UnityEngine;
using System.Collections;

public class Hair : MonoBehaviour
{
    [SerializeField] private GameObject baseObject;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private void Awake() {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void AttachToBase() {
        Transform currentParent = transform.parent;
        transform.parent = baseObject.transform;
        transform.position = Vector3.zero;
        transform.localRotation = originalLocalRotation;
        transform.localPosition = originalLocalPosition;
        transform.parent = currentParent;

        FixedJoint hairJoint = gameObject.GetComponent<FixedJoint>();
        if (!hairJoint) {
            hairJoint = gameObject.AddComponent<FixedJoint>();
        }
        hairJoint.connectedBody = baseObject.rigidbody;
    }

    public void DetatchFromBase() {
        FixedJoint hairJoint = gameObject.GetComponent<FixedJoint>();
        if (hairJoint) {
            Destroy(hairJoint);
        }
    }
}
