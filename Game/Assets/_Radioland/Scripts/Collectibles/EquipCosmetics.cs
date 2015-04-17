using UnityEngine;
using System.Collections;

public class EquipCosmetics : MonoBehaviour
{
    [SerializeField] private Renderer bodyRenderer;

    [Header("Float Hat")]
    [SerializeField] private GameObject flowerhat;

    [Header("Mustache")]
    [SerializeField] private GameObject mustache;

    [Header("Birb Outfit")]
    [SerializeField] private GameObject birbHat;
    [SerializeField] private Material birbOutfit;

    private void Awake() {
        CosmeticsManager.Register("birboutfit");
        CosmeticsManager.Register("mustache");
        CosmeticsManager.Register("flowerhat");
    }

    private void Start() {
        if (CosmeticsManager.IsEquipped("flowerhat")) { EquipFlowerHat(); }
        if (CosmeticsManager.IsEquipped("mustache")) { EquipMustache(); }
        if (CosmeticsManager.IsEquipped("birboutfit")) { EquipBirbOutfit(); }

        Messenger.AddListener<string>("UnlockCosmetic", OnUnlockCosmetic);
    }

    private void Update() {

    }

    private void OnUnlockCosmetic(string cosmeticName) {
        CosmeticsManager.Equip(cosmeticName);

        switch (cosmeticName) {
            case "flowerhat":
                EquipFlowerHat();
                break;
            case "mustache":
                EquipMustache();
                break;
            case "birboutfit":
                EquipBirbOutfit();
                break;
            default:
                break;
        }
    }

    private void EquipFlowerHat() {
        flowerhat.SetActive(true);
    }

    private void EquipMustache() {
        mustache.SetActive(true);
    }

    private void EquipBirbOutfit() {
        birbHat.SetActive(true);
        bodyRenderer.material = birbOutfit;
    }
}
