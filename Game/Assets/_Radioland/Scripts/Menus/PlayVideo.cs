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
    private Camera mainCamera;

    private const float padding = 1f;

    private void Awake() {
        Renderer myRenderer = gameObject.GetComponent<Renderer>();
        movie = (MovieTexture) myRenderer.material.mainTexture;

        source = gameObject.GetComponent<AudioSource>();

        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        float movieAspect = movie.width * 1.0f / movie.height;
        float cameraAspect = mainCamera.aspect;

        Vector2 cameraTopLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, mainCamera.nearClipPlane));
        Vector2 cameraBottomRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, mainCamera.nearClipPlane));
        float worldSpaceWidth = cameraBottomRight.x - cameraTopLeft.x;
        float worldSpaceHeight = cameraTopLeft.y - cameraBottomRight.y;

        // Rescale to fit fullscreen while keeping aspect ratio.
        float width, height;
        if (movieAspect > cameraAspect) {
            // Movie wider than screen, space at top and bottom (not full height).
            width = worldSpaceWidth;
            height = worldSpaceWidth / movieAspect;
        } else {
            // Movie taller than screen, space at left and right (not full width).
            height = worldSpaceHeight;
            width = worldSpaceHeight * movieAspect;
        }
        // Transform pivot is centered in X and Y, set scale.
        transform.localScale = new Vector3(width, height, 1f);

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
