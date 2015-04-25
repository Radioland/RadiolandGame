using UnityEngine;
using System.Collections;

public class EquipCosmetics : MonoBehaviour
{
    [SerializeField] private Renderer bodyRenderer;
    private Material originalOutfit;
    [SerializeField] private EffectManager equipEffects;
    [SerializeField] [Range(0f, 2f)] private float equipDelay = 0f;

    [Header("Float Hat")]
    [SerializeField] private GameObject flowerhat;

    [Header("Mustache")]
    [SerializeField] private GameObject mustache;

    [Header("Birb Outfit")]
    [SerializeField] private GameObject birbHat;
    [SerializeField] private Material birbOutfit;

    private string latestEquippedCosmetic;

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
        Messenger.AddListener<string>("EquipCosmetic", OnEquipCosmetic);
        Messenger.AddListener<string>("UnequipCosmetic", OnUnequipCosmetic);
    }

    private void Update() {

    }

    private void OnUnlockCosmetic(string cosmeticName) {
        // Only allow one to be equipped at a time.
        if (CosmeticsManager.IsEquipped("flowerhat")) { UnequipFlowerHat(); }
        if (CosmeticsManager.IsEquipped("mustache")) { UnequipMustache(); }
        if (CosmeticsManager.IsEquipped("birboutfit")) { UnequipBirbOutfit(); }

        CosmeticsManager.Equip(cosmeticName);
    }

    private void OnEquipCosmetic(string cosmeticName) {
        CancelInvoke("Equip");

        latestEquippedCosmetic = cosmeticName;

        Invoke("Equip", equipDelay);

        if (equipEffects) {
            equipEffects.StartEvent();
        }
    }

    private void Equip() {
        if (latestEquippedCosmetic == "flowerhat") { EquipFlowerHat(); }
        if (latestEquippedCosmetic == "mustache") { EquipMustache(); }
        if (latestEquippedCosmetic == "birboutfit") { EquipBirbOutfit(); }
    }

    private void OnUnequipCosmetic(string cosmeticName) {
        if (cosmeticName == "flowerhat") { UnequipFlowerHat(); }
        if (cosmeticName == "mustache") { UnequipMustache(); }
        if (cosmeticName == "birboutfit") { UnequipBirbOutfit(); }
    }

    #region Flower Hat
    private void EquipFlowerHat() {
        flowerhat.SetActive(true);
    }

    private void UnequipFlowerHat() {
        flowerhat.SetActive(false);
    }
    #endregion Flower Hat

    #region Mustache
    private void EquipMustache() {
        mustache.SetActive(true);
    }

    private void UnequipMustache() {
        mustache.SetActive(false);
    }
    #endregion Mustache

    #region Birb Outfit
    private void EquipBirbOutfit() {
        birbHat.SetActive(true);
        bodyRenderer.material = birbOutfit;
    }

    private void UnequipBirbOutfit() {
        birbHat.SetActive(false);
        bodyRenderer.material = originalOutfit;
    }
    #endregion Birb Outfit
}
