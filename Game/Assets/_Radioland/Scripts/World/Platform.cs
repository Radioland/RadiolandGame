using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{
    public enum SurfaceType
    {
        Generic, Grass, Rock, Metal, Plastic, Glass
    }

    public SurfaceType surfaceType = SurfaceType.Generic;

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
}
