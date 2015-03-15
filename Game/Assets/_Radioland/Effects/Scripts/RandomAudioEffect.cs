using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private List<AudioClip> audioClips;
    [Range(0.0f, 1.0f)] [SerializeField] private float volumeScale = 1.0f;

    private float secondaryScale;

    protected override void Awake() {
        base.Awake();

        secondaryScale = 1.0f;
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

        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)], volumeScale * secondaryScale);
    }

    public override void EndEffect() {
        base.EndEffect();
    }

    public void ScaleVolume(float volumeScaleModifier) {
        secondaryScale = volumeScaleModifier;
    }
}
