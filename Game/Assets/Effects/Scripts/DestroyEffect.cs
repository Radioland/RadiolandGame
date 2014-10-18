using UnityEngine;
using System.Collections;

public class DestroyEffect : Effect
{
    public GameObject objectToDestroy;
    public bool defaultToSelf = false;
    
    protected override void Awake() {
        base.Awake();

        if (!objectToDestroy && defaultToSelf) {
            objectToDestroy = gameObject;
        }
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

        if (objectToDestroy) {
            Destroy(objectToDestroy);
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
