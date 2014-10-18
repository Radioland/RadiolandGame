using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioEffect : Effect
{
    public List<AudioClip> audioClips;
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
            audio.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)], volumeScale);
            lastTimePlayed = Time.time;
        } else {
            EndEffect();
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
