using UnityEngine;
using System.Collections;

static internal class CosmeticsUnlockedInitializer
{
    //Disable the unused variable warning
#pragma warning disable 0414
    static private CosmeticsUnlocked cosmeticsUnlocked = (new GameObject("CosmeticsUnlocked")).AddComponent<CosmeticsUnlocked>();
#pragma warning restore 0414

    static public void Unlock(string collectibleName) {
        switch (collectibleName) {
            case "flowerhat":
                cosmeticsUnlocked.flowerHatState = CosmeticsUnlocked.UnlockState.Unlocked;
                break;
            case "mustache":
                cosmeticsUnlocked.mustacheState = CosmeticsUnlocked.UnlockState.Unlocked;
                break;
            case "birbhat":
                cosmeticsUnlocked.birbHatState = CosmeticsUnlocked.UnlockState.Unlocked;
                break;
            default:
                Debug.Log("Unknown collectible name:" + collectibleName);
                break;
        }
    }
}

public class CosmeticsUnlocked : MonoBehaviour
{
    public enum UnlockState
    {
        Locked, Unlocked, Equipped
    }

    public UnlockState flowerHatState = UnlockState.Locked;
    public UnlockState mustacheState = UnlockState.Locked;
    public UnlockState birbHatState = UnlockState.Locked;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {

    }

    private void Update() {

    }
}
