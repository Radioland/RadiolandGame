using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static internal class CosmeticsManager
{
    public enum UnlockState
    {
        Locked, Unlocked, Equipped, UnknownCosmetic
    }

    private static Dictionary<string, UnlockState> cosmeticStates = new Dictionary<string, UnlockState>();

    static public void Register(string cosmeticName) {
        if (!cosmeticStates.ContainsKey(cosmeticName)) {
            cosmeticStates.Add(cosmeticName, UnlockState.Locked); // Default state.
        }
    }

    static public void Unlock(string cosmeticName) {
        cosmeticStates[cosmeticName] = UnlockState.Unlocked;
        Messenger.Broadcast("UnlockCosmetic", cosmeticName);
    }

    static public void Equip(string cosmeticName) {
        UnequipAll();
        cosmeticStates[cosmeticName] = UnlockState.Equipped;
        Messenger.Broadcast("EquipCosmetic", cosmeticName);
    }

    static public void Unequip(string cosmeticName) {
        UnlockState unlockState;
        if (cosmeticStates.TryGetValue(cosmeticName, out unlockState)) {
            if (unlockState == UnlockState.Equipped) {
                cosmeticStates[cosmeticName] = UnlockState.Unlocked;
                Messenger.Broadcast("UnequipCosmetic", cosmeticName);
            }
        }
    }

    static public void UnequipAll() {
        List<string> keys = new List<string>(cosmeticStates.Keys);
        foreach (string key in keys) {
            Unequip(key);
        }
    }

    static public UnlockState GetState(string cosmeticName) {
        UnlockState unlockState;
        return cosmeticStates.TryGetValue(cosmeticName, out unlockState) ? unlockState : UnlockState.UnknownCosmetic;
    }

    static public bool IsNotLocked(string cosmeticName) {
        UnlockState unlockState;
        return cosmeticStates.TryGetValue(cosmeticName, out unlockState) && unlockState != UnlockState.Locked;
    }

    static public bool IsEquipped(string cosmeticName) {
        UnlockState unlockState;
        return cosmeticStates.TryGetValue(cosmeticName, out unlockState) && unlockState == UnlockState.Equipped;
    }
}
