using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public GameObject target;
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private Vector3 originalEulerAngles;

    private void Awake()
    {
        // if no object referenced, grab the main camera
        if (!target) {
            target = Camera.main.gameObject;
        }

        originalEulerAngles = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        transform.LookAt(target.transform);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        if (lockX) { eulerAngles.x = originalEulerAngles.x; }
        if (lockY) { eulerAngles.y = originalEulerAngles.y; }
        if (lockZ) { eulerAngles.z = originalEulerAngles.z; }
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
