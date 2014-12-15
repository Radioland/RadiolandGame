using UnityEngine;
using System.Collections;

public class WindPushArea : MonoBehaviour
{
    [SerializeField] private Vector3 windVelocityLocal;
    public Vector3 windVelocity {
        get {
            return (transform.forward * windVelocityLocal.z +
                    transform.right * windVelocityLocal.x +
                    transform.up * windVelocityLocal.y);
        }
    }

    void Awake() {

    }

    void Start() {

    }

    void Update() {

    }

    void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, windVelocity.normalized);
    }
}
