using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class HideChangeMaterial : MonoBehaviour
{
    [SerializeField] private Material hideMaterial;

    private Renderer myRenderer;
    private Material originalMaterial;

    private void Awake() {
        myRenderer = gameObject.GetComponent<Renderer>();
        originalMaterial = myRenderer.material;

        // Apply hideMaterial and convert it to an instance.
        myRenderer.material = hideMaterial;
        myRenderer.material.SetTextureOffset("_MainTex", myRenderer.material.GetTextureOffset("_MainTex"));
        hideMaterial = myRenderer.material;

        myRenderer.material = originalMaterial;
    }

    private void Start() {

    }

    private void Update() {

    }

    public void Hide() {
        myRenderer.material = hideMaterial;
    }

    public void UnHide() {
        myRenderer.material = originalMaterial;
    }
}
