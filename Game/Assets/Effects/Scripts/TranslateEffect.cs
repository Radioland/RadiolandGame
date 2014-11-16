using UnityEngine;
using System.Collections;

public class TranslateEffect : Effect
{
    [SerializeField] private Vector3 finalTranslationDelta;

    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();

        if (hasStarted) {
            transform.position = transform.position + finalTranslationDelta / duration * Time.deltaTime;
        }
    }
    
    public override void TriggerEffect() {
        base.TriggerEffect();
    }
    
    public override void StartEffect() {
        base.StartEffect();
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
