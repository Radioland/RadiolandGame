using UnityEngine;
using System.Collections;

public class LoadLevelEffect : Effect
{
    public int levelToLoad;
    
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

        Application.LoadLevel(levelToLoad);
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
