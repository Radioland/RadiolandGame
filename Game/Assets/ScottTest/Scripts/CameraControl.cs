using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public GameObject player;

    // The camera moves around a sphere centered on the player.
    // The radius is affected by the current zoom level.
    // The two angles are controlled by mouse input.
    [Range(5.0f, 30.0f)] public float radius = 10.0f;
    public float verticalAngle = 20.0f; // X-Axis Euler Angle
    public float horizontalAngle = 0.0f; // Y-Axis Euler Angle

    void Awake() {
        if (!player) { player = GameObject.FindWithTag("Player"); }
    }

    void Start() {

    }

    void Update() {

    }

    void LateUpdate() {
        // The default (no mouse buttons) behavior is to follow behind the player.
        // Rotate the horizontalAngle towards the player's orientation.
        // Maintain a constant verticalAngle.

        Vector3 position = player.transform.position;
        position -= player.transform.forward * radius;

        // Adjust this depending on the model's orientation.
        horizontalAngle = -player.transform.eulerAngles.y - 90;

        float phi = horizontalAngle * Mathf.Deg2Rad;
        float theta = verticalAngle * Mathf.Deg2Rad;

        // Calculate the point on the unit sphere with the provided angles.
        float x = Mathf.Cos(theta) * Mathf.Cos(phi);
        float y = Mathf.Sin(theta);
        float z = Mathf.Cos(theta) * Mathf.Sin(phi);
        // Offset from the player by the vector to that point at the given radius.
        Vector3 offset = new Vector3(x, y, z);
        transform.position = player.transform.position + offset * radius;

        transform.LookAt(player.transform.position);
    }
}
