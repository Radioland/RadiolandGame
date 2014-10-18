using UnityEngine;
using System.Collections;

public class EnableTagEffect : Effect
{
    public string tagToEnable;
    
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

        GameObject [] tagObjects = GameObject.FindGameObjectsWithTag(tagToEnable);
        foreach (GameObject tagObject in tagObjects) {
            Renderer[] tagObjectRenderers = tagObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer tagObjectRenderer in tagObjectRenderers) {
                tagObjectRenderer.enabled = true;
            }
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
