using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    [System.Flags]
    public enum SurfaceType
    {
        Generic = (1 << 0),
        Grass = (1 << 1),
        Rock = (1 << 2),
        Metal = (1 << 3),
        Plastic = (1 << 4),
        Glass = (1 << 5)
    }

    public SurfaceType surfaceType = SurfaceType.Generic;
    public bool pushPlayerJumping = true;
    public bool pushPlayerGrounded = true;

    private Vector3 m_lastVelocity;
    public Vector3 lastVelocity { get { return m_lastVelocity; } }

    public Vector3 deltaPosition { get { return transform.position - lastPosition; } }

    private Vector3 lastPosition;

    private void Awake() {
        m_lastVelocity = Vector3.zero;
    }

    private void Start() {

    }

    private void LateUpdate() {
        m_lastVelocity = deltaPosition / Time.deltaTime;
        lastPosition = transform.position;
    }

    public static Platform GetPlatformOnObject(GameObject theObject) {
        Platform platform = theObject.GetComponent<Platform>();
        if (platform) { return platform; }

        if (theObject.transform.parent) {
            platform = theObject.transform.parent.GetComponent<Platform>();
            if (platform) { return platform; }
        }

        return null;
    }
}
