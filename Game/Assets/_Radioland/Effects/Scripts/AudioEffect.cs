using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private AudioClip audioClip;
    [Range(0.0f, 1.0f)] [SerializeField] private float volumeScale = 1.0f;

    private float lastTimePlayed;
    private AudioSource myAudioSource;

    protected override void Awake() {
        base.Awake();

        myAudioSource = gameObject.GetComponent<AudioSource>();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        if (audioClip) {
            myAudioSource.clip = audioClip;
        }
        myAudioSource.volume = volumeScale;
        myAudioSource.Play();
    }

    public override void EndEffect() {
        base.EndEffect();

        myAudioSource.Stop();
    }
}
