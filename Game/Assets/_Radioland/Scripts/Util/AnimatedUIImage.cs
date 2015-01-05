using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AnimatedUIImage : MonoBehaviour
{
    [SerializeField] private float timeBetweenFrames = 1.0f;
    [SerializeField] private List<Sprite> sprites;

    private Image image;

    private void Awake() {
        image = gameObject.GetComponent<Image>();
    }

    private void Start() {

    }

    private void Update() {
        if (timeBetweenFrames > 0.0f && sprites.Count > 0) {
            int index = Mathf.FloorToInt(Time.time / timeBetweenFrames) % sprites.Count;
            image.sprite = sprites[index];
        }
    }
}
