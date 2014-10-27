using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    private Vector3 m_lastVelocity;
    public Vector3 lastVelocity { get { return m_lastVelocity; } }

    public Vector3 deltaPosition { get { return transform.position - lastPosition; } }

    private Vector3 lastPosition;

    void Awake() {
        m_lastVelocity = Vector3.zero;
    }

    void Start() {

    }

    void LateUpdate() {
        m_lastVelocity = deltaPosition / Time.deltaTime;
        lastPosition = transform.position;
    }
}
