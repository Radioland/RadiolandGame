using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FadeGlass : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1f;

    private Renderer myRenderer;
    private Material myMaterial;

    private bool fadingOut;
    private bool fadingIn;

    private float currentAlpha;
    private float alphaVelocity;

    private void Awake() {
        myRenderer = gameObject.GetComponent<Renderer>();
        myMaterial = myRenderer.material;

        fadingOut = false;
        fadingIn = false;
    }

    private void Start() {

    }

    private void Update() {
        if (fadingOut) {
            currentAlpha = Mathf.SmoothDamp(currentAlpha, 1f, ref alphaVelocity, fadeTime);

            if (currentAlpha > 0.99f) {
                currentAlpha = 1f;
                fadingOut = false;
            }
        }

        if (fadingIn) {
            currentAlpha = Mathf.SmoothDamp(currentAlpha, 0f, ref alphaVelocity, fadeTime);

            if (currentAlpha < 0.01f) {
                currentAlpha = 0f;
                fadingIn = false;
            }
        }

        myMaterial.SetFloat("_AlphaLevel", currentAlpha);
    }

    public void StartFadeOut() {
        if (fadingOut) { return; }

        fadingOut = true;
        fadingIn = false;

        alphaVelocity = 0f;
    }

    public void StartFadeIn() {
        if (fadingIn) { return; }

        fadingOut = false;
        fadingIn = true;

        alphaVelocity = 0f;
    }
}
