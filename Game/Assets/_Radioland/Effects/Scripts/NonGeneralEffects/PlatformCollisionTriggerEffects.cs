using UnityEngine;
using System.Collections;

public class PlatformCollisionTriggerEffects : CollisionTriggerEffects
{
    [SerializeField] [BitMask(typeof(Platform.SurfaceType))]
    private Platform.SurfaceType surfaceType;

    protected override void StartEventIfMatch(GameObject collisionObject) {
        if (testColliderTag.Length == 0 || collisionObject.tag.Equals(testColliderTag)) {
            Platform platform = GetPlatformOnObject(collisionObject);
            if (platform && ((platform.surfaceType & surfaceType) > 0)) {
                StartEvent();
            }
        }
    }

    private Platform GetPlatformOnObject(GameObject theObject) {
        Platform platform = theObject.GetComponent<Platform>();
        if (platform) { return platform; }

        if (theObject.transform.parent) {
            platform = theObject.transform.parent.GetComponent<Platform>();
            if (platform) { return platform; }
        }

        return null;
    }
}
