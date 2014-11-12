using UnityEngine;
using System.Collections;

public class EnableObjectEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private GameObject objectToEnable;
    [SerializeField] private bool disableAtStart = true;
    [SerializeField] private bool disableOnEnd = true;
    
    protected override void Awake() {
        base.Awake();
        
        if (objectToEnable && disableAtStart) {
            objectToEnable.SetActive(false);
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
        
        if (objectToEnable) {
            objectToEnable.SetActive(true);
        }
    }
    
    public override void EndEffect() {
        base.EndEffect();

        if (objectToEnable && disableOnEnd) {
            objectToEnable.SetActive(false);
        }
    }
}
