using UnityEngine;
using System.Collections;

public class UnlockCosmeticEffect : Effect
{
    [SerializeField] private string cosmeticName;

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

        CosmeticsManager.Register(cosmeticName);
        CosmeticsManager.Unlock(cosmeticName);

        Messenger.Broadcast("UnlockCosmetic", cosmeticName);
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
