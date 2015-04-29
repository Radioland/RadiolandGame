using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class PlayVideo : MonoBehaviour
{
    [SerializeField] private SimpleMenu menu;
    [SerializeField] private float duration;

    private MovieTexture movie;
    private AudioSource source;

    private const float padding = 1f;

    private void Awake() {
        Renderer myRenderer = gameObject.GetComponent<Renderer>();
        movie = (MovieTexture) myRenderer.material.mainTexture;

        source = gameObject.GetComponent<AudioSource>();

        movie.Play();
        source.Play();
    }

    private void Start() {

    }

    private void Update() {
        if (Time.time > duration + padding || Input.GetButtonDown("Submit")) {
            menu.LoadNextLevel();
        }
    }
}
