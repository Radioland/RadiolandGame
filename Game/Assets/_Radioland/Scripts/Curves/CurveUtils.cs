using UnityEngine;

public class CurveUtils
{
    public static void GetUpAndRight(Vector3 forward, out Vector3 up, out Vector3 right) {
        up = Vector3.Cross(forward, Vector3.up).normalized;
        if (up.magnitude == 0) {
            up = Vector3.Cross(forward, Vector3.forward).normalized;
        }
        right = Vector3.Cross(forward, up).normalized;
    }
}
