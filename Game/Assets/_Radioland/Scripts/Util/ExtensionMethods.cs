using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    // -----------------------------------------------------------------------------------------------
    // Restrict the magnitude of a Vector3 from min to max, returns the new Vector3
    public static Vector3 RestrictMagnitude(this Vector3 inputVector, float min, float max) {
        Vector3 returnVector = inputVector;
        if (returnVector.magnitude < min) { returnVector = returnVector.normalized * min; }
        if (returnVector.magnitude > max) { returnVector = returnVector.normalized * max; }
        return returnVector;
    }
    // -----------------------------------------------------------------------------------------------

    // -----------------------------------------------------------------------------------------------
    // GetPath from http://answers.unity3d.com/questions/8500/how-can-i-get-the-full-path-to-a-gameobject.html
    // Returns a string representation of a Component or Transform's "path" within the scene
    public static string GetPath(this Transform current) {
        if (current.parent == null)
            return "/" + current.name;
        return current.parent.GetPath() + "/" + current.name;
    }

    public static string GetPath(this Component component) {
        return component.transform.GetPath() + "/" + component.GetType().ToString();
    }
    // -----------------------------------------------------------------------------------------------
}
