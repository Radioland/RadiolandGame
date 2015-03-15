using UnityEngine;
using System.Collections;

public class AnimateUVs : MonoBehaviour {
    public Vector2 scrollSpeed = new Vector2 (0.0f, 0.5F);

    private Renderer rendererToChange;

    private void Awake() {
        rendererToChange = gameObject.GetComponent<Renderer>();
    }

    private void Update() {
        Vector2 offset = Time.time * scrollSpeed;
        rendererToChange.material.SetTextureOffset("_MainTex", offset);
    }
}
