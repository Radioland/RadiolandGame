using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextMessageEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private string message;
    [SerializeField] private Text text;
    
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
        
        if (text) {
            text.text = message;
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
