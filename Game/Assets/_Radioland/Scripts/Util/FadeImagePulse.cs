using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeImagePulse : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private Interpolate.EaseType easeType = Interpolate.EaseType.EaseInOutQuad;

    private Interpolate.Function easeFunction;
    private Color originalColor;

    private void Reset() {
        image = gameObject.GetComponent<Image>();
    }

    private void Awake() {
        easeFunction = Interpolate.Ease(easeType);
        originalColor = image.color;
    }

    private void Start() {

    }

    private void Update() {
        float t = Mathf.PingPong(Time.time, duration) / duration;
        t = easeFunction(minAlpha, 1f, t, 1f);

        Color color = originalColor;
        color.a = t;
        image.color = color;
    }
}
