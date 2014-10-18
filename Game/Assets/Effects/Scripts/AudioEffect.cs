using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioEffect : Effect
{
    public AudioClip audioClip;
    [Range(0.0f, 1.0f)] public float volumeScale = 1.0f;
    public float cooldown = 0.0f;

    private float lastTimePlayed;

    protected override void Awake() {
        base.Awake();
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

        if (Time.time - lastTimePlayed > cooldown) {
            audio.PlayOneShot(audioClip, volumeScale);
            lastTimePlayed = Time.time;
        } else {
            EndEffect();
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
