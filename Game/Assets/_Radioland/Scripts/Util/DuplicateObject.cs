using UnityEngine;
using System.Collections;

public class DuplicateObject : MonoBehaviour
{
    public bool flipPosX;
    public bool flipPosY;
    public bool flipPosZ;

    public bool flipRotX;
    public bool flipRotY;
    public bool flipRotZ;

    // Note: this should run as Start and _not_ Awake since this script is
    // Destroyed on the duplicate object to stop recursion.
    private void Start() {
        Vector3 pos = transform.localPosition;
        if (flipPosX) { pos.x *= -1; }
        if (flipPosY) { pos.y *= -1; }
        if (flipPosZ) { pos.z *= -1; }

        Vector3 rot = transform.localRotation.eulerAngles;
        if (flipRotX) { rot.x += 180; }
        if (flipRotY) { rot.y += 180; }
        if (flipRotZ) { rot.z += 180; }

        GameObject duplicate = (GameObject) Instantiate(gameObject);
        duplicate.transform.SetParent(transform.parent, false);
        duplicate.transform.localPosition = pos;
        duplicate.transform.localRotation = Quaternion.Euler(rot);
        Destroy(duplicate.GetComponent<DuplicateObject>());
    }
}
