using UnityEngine;
using System.Collections;
using System.Linq;

public class PlatformLandingTriggerEffects : TriggerEffects
{
    [SerializeField] [BitMask(typeof(Platform.SurfaceType))]
    private Platform.SurfaceType surfaceType;
    [SerializeField] private LayerMask layerMask;

    private const float raycastDistance = 0.2f;

    protected override void Awake() {
        base.Awake();

        Messenger.AddListener<float>("Grounded", OnGrounded);
    }

    private void OnGrounded(float verticalSpeed) {
        Platform platform = GetPlatformUnder();
        if (platform && ((platform.surfaceType & surfaceType) > 0)) {
            StartEvent();
        }
    }

    private Platform GetPlatformUnder() {

        Vector3 start = transform.position + Vector3.up; // Adds 1.0f up.
        float distance = 1.0f + raycastDistance; // Counteracts the 1.0f up.

        Debug.DrawLine(start, start + Vector3.down * distance, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(start, Vector3.down, distance, layerMask).OrderBy(h=>h.distance).ToArray();
        return hits.Select(hit => Platform.GetPlatformOnObject(hit.collider.gameObject)).FirstOrDefault(platform => platform);
    }
}
