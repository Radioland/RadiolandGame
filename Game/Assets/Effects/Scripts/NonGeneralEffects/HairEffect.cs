using UnityEngine;
using System.Collections;

public class HairEffect : Effect
{
    // Variables to specify in the editor.
    public Hair hair;
    
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

        if (hair) {
            hair.DetatchFromBase();
            hair.gameObject.SetActive(false);
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
        
        if (hair) {
            hair.gameObject.SetActive(true);
            hair.AttachToBase();
        }
    }
}
