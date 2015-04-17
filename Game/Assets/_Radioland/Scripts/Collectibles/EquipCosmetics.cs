using UnityEngine;
using System.Collections;

public class EquipCosmetics : MonoBehaviour
{
    [SerializeField] private Renderer bodyRenderer;
    private Material originalOutfit;

    [Header("Float Hat")]
    [SerializeField] private GameObject flowerhat;

    [Header("Mustache")]
    [SerializeField] private GameObject mustache;

    [Header("Birb Outfit")]
    [SerializeField] private GameObject birbHat;
    [SerializeField] private Material birbOutfit;

    private void Awake() {
        CosmeticsManager.Register("flowerhat");
        CosmeticsManager.Register("mustache");
        CosmeticsManager.Register("birboutfit");

        originalOutfit = bodyRenderer.material;
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
        // Only allow one to be equipped at a time.
        if (CosmeticsManager.IsEquipped("flowerhat")) { UnEquipFlowerHat(); }
        if (CosmeticsManager.IsEquipped("mustache")) { UnEquipMustache(); }
        if (CosmeticsManager.IsEquipped("birboutfit")) { UnEquipBirbOutfit(); }

        CosmeticsManager.Equip(cosmeticName);

        if (cosmeticName == "flowerhat") { EquipFlowerHat(); }
        if (cosmeticName == "mustache") { EquipMustache(); }
        if (cosmeticName == "birboutfit") { EquipBirbOutfit(); }
    }

    #region Flower Hat
    private void EquipFlowerHat() {
        flowerhat.SetActive(true);
    }

    private void UnEquipFlowerHat() {
        flowerhat.SetActive(false);
    }
    #endregion Flower Hat

    #region Mustache
    private void EquipMustache() {
        mustache.SetActive(true);
    }

    private void UnEquipMustache() {
        mustache.SetActive(false);
    }
    #endregion Mustache

    #region Birb Outfit
    private void EquipBirbOutfit() {
        birbHat.SetActive(true);
        bodyRenderer.material = birbOutfit;
    }

    private void UnEquipBirbOutfit() {
        birbHat.SetActive(false);
        bodyRenderer.material = originalOutfit;
    }
    #endregion Birb Outfit
}
