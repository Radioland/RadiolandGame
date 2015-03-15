using UnityEngine;
using System.Collections;

public class LinkRendererToLight : MonoBehaviour
{
    public Light theLight;
    public string shaderColorName = "_Color";

    private Renderer rendererToChange;

    private void Awake() {
        rendererToChange = gameObject.GetComponent<Renderer>();

        if (theLight == null) {
            theLight = gameObject.GetComponentInParent<Light>();
        }
        if (!rendererToChange.material.HasProperty(shaderColorName)) {
            Debug.LogWarning(rendererToChange.GetPath() + " has no material color named " + shaderColorName);
        }
    }

    // Update is called once per frame
    private void Update() {
        if (!rendererToChange.material.HasProperty(shaderColorName)) { return; }

        if (rendererToChange.material.GetColor(shaderColorName) != theLight.color) {
            rendererToChange.material.SetColor(shaderColorName, theLight.color);
        }
    }
}
