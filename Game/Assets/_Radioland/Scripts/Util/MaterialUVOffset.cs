using UnityEngine;
using System.Collections;

public class MaterialUVOffset : MonoBehaviour
{
    public Vector2 offset;
    public bool random;
    public string shaderColorName = "_MainTex";

    private void Awake() {
        if (random) {
            offset.x = Random.Range(0f, 1f);
            offset.y = Random.Range(0f, 1f);
        }

        Renderer rendererToChange = gameObject.GetComponent<Renderer>();

        if (!rendererToChange.material.HasProperty(shaderColorName)) {
            Debug.LogWarning(rendererToChange.GetPath() + " has no material color named " + shaderColorName);
        } else {
            rendererToChange.material.SetTextureOffset(shaderColorName, offset);
        }
    }
}
