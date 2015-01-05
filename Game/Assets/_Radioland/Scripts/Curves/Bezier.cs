// Reference: http://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEngine;

public class Bezier
{
    /// <summary>
    /// Get a point on a quadratic Bezier curve.
    /// </summary>
    /// <param name="p0">The start anchor point.</param>
    /// <param name="p1">The middle (control) point.</param>
    /// <param name="p2">The end anchor point.</param>
    /// <param name="t">Time value along the curve.</param>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        t = Mathf.Clamp01(t);

        // Intuitive representation using lerps.
        //return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);

        // B(t) = (1 - t)^2 p0 + 2(1 - t)t p1 + t^2 p2
        float oneMinusT = 1f - t;
        return (oneMinusT * oneMinusT * p0 +
                2f * oneMinusT * t * p1 +
                t * t * p2);
    }

    /// <summary>
    /// Get the first derivative at a point on a quadratic Bezier curve.
    /// </summary>
    /// <param name="p0">The start anchor point.</param>
    /// <param name="p1">The middle (control) point.</param>
    /// <param name="p2">The end anchor point.</param>
    /// <param name="t">Time value along the curve.</param>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        // B'(t) = 2(1 - t)(p1 - p0) + 2t(p2 - p1)
        return (2f * (1f - t) * (p1 - p0) +
                2f * t * (p2 - p1));
    }

    /// <summary>
    /// Get a point on a cubic Bezier curve.
    /// </summary>
    /// <param name="p0">The start anchor point.</param>
    /// <param name="p1">The first control point.</param>
    /// <param name="p2">The second control point.</param>
    /// <param name="p3">The end anchor point.</param>
    /// <param name="t">Time value along the curve.</param>
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return (oneMinusT * oneMinusT * oneMinusT * p0 +
                3f * oneMinusT * oneMinusT * t * p1 +
                3f * oneMinusT * t * t * p2 +
                t * t * t * p3);
    }

    /// <summary>
    /// Get the first derivative at a point on a cubic Bezier curve.
    /// </summary>
    /// <param name="p0">The start anchor point.</param>
    /// <param name="p1">The first control point.</param>
    /// <param name="p2">The second control point.</param>
    /// <param name="p3">The end anchor point.</param>
    /// <param name="t">Time value along the curve.</param>
    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return (3f * oneMinusT * oneMinusT * (p1 - p0) +
                6f * oneMinusT * t * (p2 - p1) +
                3f * t * t * (p3 - p2));
    }
}
