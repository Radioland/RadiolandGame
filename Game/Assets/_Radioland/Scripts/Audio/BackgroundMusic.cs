using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] private float maxVolume;

    private RadioControl radioControl;
    private AudioSource audioSource;

    private void Awake() {
        GameObject player = GameObject.FindWithTag("Player");
        radioControl = player.GetComponentInChildren<RadioControl>();

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start() {

    }

    private void Update() {
        if (radioControl && audioSource) {
            audioSource.volume = radioControl.backgroundStrength * maxVolume;
        }
    }
}
