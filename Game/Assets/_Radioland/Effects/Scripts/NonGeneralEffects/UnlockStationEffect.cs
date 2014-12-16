using UnityEngine;
using System.Collections;

public class UnlockStationEffect : Effect
{
    // Variables to specify in the editor.
    [SerializeField] private int stationId;
    
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

        GameObject player = GameObject.FindWithTag("Player");
        RadioControl radioControl = player.GetComponentInChildren<RadioControl>();
        radioControl.UnlockStation(stationId);
    }
    
    public override void EndEffect() {
        base.EndEffect();
    }
}
