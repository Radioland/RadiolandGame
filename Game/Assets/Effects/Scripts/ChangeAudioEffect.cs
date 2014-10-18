using UnityEngine;
using System.Collections;

public class ChangeAudioEffect : Effect
{
    //public AudioSource sourceToChange;
    public GameObject objectWithSource;
    public AudioClip clipToChangeTo;

    private AudioSource audioSourceOld;
    private AudioSource audioSourceNew;
    private float maxVolume;
    
    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();

        if (hasStarted && isPlaying) {
            audioSourceOld.volume = Mathf.Lerp(maxVolume, 0.0f, percentDurationElapsed);
            audioSourceNew.volume = Mathf.Lerp(0.0f, maxVolume, percentDurationElapsed);

            if (percentDurationElapsed >= 0.99f) {
                EndEffect();
            }
        }
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();

        audioSourceOld = objectWithSource.GetComponent<AudioSource>();
        audioSourceNew = objectWithSource.AddComponent<AudioSource>();
        maxVolume = audioSourceOld.volume;
        audioSourceNew.priority = audioSourceOld.priority;
        audioSourceNew.loop = audioSourceOld.loop;

        audioSourceNew.clip = clipToChangeTo;
        audioSourceNew.volume = 0;
        audioSourceNew.Play();
    }
    
    public override void EndEffect() {
        base.EndEffect();

        audioSourceNew.volume = maxVolume;

        Destroy(audioSourceOld);
    }
}
