// Reference: http://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEngine;
using System;

public enum BezierControlPointMode
{
    Free,
    Aligned,
    Mirrored
}

public class BezierSpline : MonoBehaviour
{
    #region Internal representation.
    [SerializeField] private Vector3[] points;
    [SerializeField] private BezierControlPointMode[] modes;
    [SerializeField] private bool m_loop;
    #endregion Internal representation.

    private const int gizmoStepsPerCurve = 10;

    public void Reset() {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };

        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Mirrored,
            BezierControlPointMode.Mirrored
        };
    }

    #region Public interface for points.
    public int ControlPointCount { get { return points.Length; } }
    public Vector3 GetControlPoint(int index) { return points[index]; }
    public void SetControlPoint(int index, Vector3 point) {
        // Move surrounding points with middle points, avoiding out of range errors.
        if (index % 3 == 0) {
            Vector3 delta = point - points[index];
            if (loop) {
                if (index == 0) {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                } else if (index == points.Length - 1) {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                } else {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            } else {
                if (index > 0) {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length) {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }
    public int GetAnchorPointIndex(int index) { return (index + 1) / 3 * 3; }
    public bool IsAnchorPoint(int index) { return index == GetAnchorPointIndex(index); }
    public int CurveCount { get { return (points.Length - 1) / 3; } }
    #endregion Public interface for points.

    #region Public interface for point modes.
    // Example: point indices [0..6] -> mode indices [0, 0, 1, 1, 1, 2, 2].
    public BezierControlPointMode GetControlPointMode(int index) {
        return modes[(index + 1) / 3];
    }
    public void SetControlPointMode(int index, BezierControlPointMode mode) {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop) {
            if (modeIndex == 0) {
                modes[modes.Length - 1] = mode;
            } else if (modeIndex == modes.Length - 1) {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }
    #endregion Public interface for point modes.

    #region Public interface for looping.
    public bool loop {
        get {
            return m_loop;
        }
        set {
            m_loop = value;
            if (m_loop) {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }
    #endregion Public interface for looping.

    #region Spline continuous property accessors.
    public Vector3 GetPoint(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetPoint(
               points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t) {
        int i;
        if (t >= 1f) {
            t = 1f;
            i = points.Length - 4;
        } else {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetFirstDerivative(
               points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t) {
        return GetVelocity(t).normalized;
    }
    #endregion Spline continuous property accessors.

    public void AddCurve() {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop) {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    /// <summary>
    /// Enforces free, aligned, or mirrored constraints on the spline.
    /// </summary>
    /// <param name="index">The fixed index under review. Its pair index may be moved.</param>
    private void EnforceMode(int index) {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free ||
            !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
            return;
        }
        // The mode must now be either Aligned or Mirrored.

        int middleIndex = modeIndex * 3;
        int fixedIndex;
        int enforcedIndex;
        if (index <= middleIndex) {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
            // Fix wrap around when looping.
            if (fixedIndex < 0) { fixedIndex = points.Length - 2; }
            if (enforcedIndex >= points.Length) { enforcedIndex = 1; }
        } else {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
            // Fix wrap around when looping.
            if (fixedIndex >= points.Length) { enforcedIndex = 1; }
            if (enforcedIndex < 0) { enforcedIndex = points.Length - 2; }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned) {
            enforcedTangent = enforcedTangent.normalized *
                              Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        int steps = gizmoStepsPerCurve * CurveCount;
        for (int i = 0; i < steps; i++) {
            Gizmos.DrawLine(GetPoint(i / (float)steps), GetPoint((i + 1) / (float)steps));
        }
    }
}
