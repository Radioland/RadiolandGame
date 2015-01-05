using UnityEngine;
using System.Collections;

public class AnimateUVs : MonoBehaviour {
    public Vector2 scrollSpeed = new Vector2 (0.0f, 0.5F);
    private void Update() {
        Vector2 offset = Time.time * scrollSpeed;
        renderer.material.SetTextureOffset("_MainTex", offset);
    }
}
