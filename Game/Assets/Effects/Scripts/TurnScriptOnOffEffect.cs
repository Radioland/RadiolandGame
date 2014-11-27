using UnityEngine;
using System.Collections;

public class TurnScriptOnOffEffect : Effect
{
    // Variables to specify in the editor.
    public MonoBehaviour script;
    [SerializeField] private bool startEnabled = true;
    [SerializeField] private bool endEnabled = true;
    
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
        
        if (script) {
            script.enabled = startEnabled;
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        if (script) {
            script.enabled = endEnabled;
        }
    }
}
