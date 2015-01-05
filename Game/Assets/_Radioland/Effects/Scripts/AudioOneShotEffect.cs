﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioOneShotEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private AudioClip audioClip;
    [Range(0.0f, 1.0f)] [SerializeField] private float volumeScale = 1.0f;

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

        audio.PlayOneShot(audioClip, volumeScale);
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
