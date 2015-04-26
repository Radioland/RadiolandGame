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

    [Header("Kimi")]
    [SerializeField] private GameObject kimi;

    [Header("Nerd Outfit")]
    [SerializeField] private GameObject nerdGlasses;
    [SerializeField] private GameObject nerdOutfit;

    private string latestEquippedCosmetic;

    private void Awake() {
        CosmeticsManager.Register("flowerhat");
        CosmeticsManager.Register("mustache");
        CosmeticsManager.Register("birboutfit");
        CosmeticsManager.Register("kimi");
        CosmeticsManager.Register("nerdoutfit");

        originalOutfit = bodyRenderer.material;
    }

    private void Start() {
        if (CosmeticsManager.IsEquipped("flowerhat")) { EquipFlowerHat(); }
        if (CosmeticsManager.IsEquipped("mustache")) { EquipMustache(); }
        if (CosmeticsManager.IsEquipped("birboutfit")) { EquipBirbOutfit(); }
        if (CosmeticsManager.IsEquipped("kimi")) { EquipKimi(); }
        if (CosmeticsManager.IsEquipped("nerdoutfit")) { EquipNerdOutfit(); }

        Messenger.AddListener<string>("UnlockCosmetic", OnUnlockCosmetic);
        Messenger.AddListener<string>("EquipCosmetic", OnEquipCosmetic);
        Messenger.AddListener<string>("UnequipCosmetic", OnUnequipCosmetic);
    }

    private void Update() {
        // Unlock all cosmetics cheat.
        if (Input.GetKeyDown(KeyCode.U)) {
            CosmeticsManager.Unlock("flowerhat");
            CosmeticsManager.Unlock("mustache");
            CosmeticsManager.Unlock("birboutfit");
            CosmeticsManager.Unlock("kimi");
            CosmeticsManager.Unlock("nerdoutfit");

            UnequipAll();
        }
    }

    private void OnUnlockCosmetic(string cosmeticName) {
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
        if (latestEquippedCosmetic == "kimi") { EquipKimi(); }
        if (latestEquippedCosmetic == "nerdoutfit") { EquipNerdOutfit(); }
    }

    private void OnUnequipCosmetic(string cosmeticName) {
        if (cosmeticName == "flowerhat") { UnequipFlowerHat(); }
        if (cosmeticName == "mustache") { UnequipMustache(); }
        if (cosmeticName == "birboutfit") { UnequipBirbOutfit(); }
        if (cosmeticName == "kimi") { UnequipKimi(); }
        if (cosmeticName == "nerdoutfit") { UnequipNerdOutfit(); }
    }

    private void UnequipAll() {
        CancelInvoke("Equip");
        CosmeticsManager.UnequipAll();
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

    #region Kimi
    private void EquipKimi() {
        kimi.SetActive(true);
    }

    private void UnequipKimi() {
        kimi.SetActive(false);
    }
    #endregion Kimi

    #region Nerd Outfit
    private void EquipNerdOutfit() {
        nerdGlasses.SetActive(true);
        nerdOutfit.SetActive(true);
    }

    private void UnequipNerdOutfit() {
        nerdGlasses.SetActive(false);
        nerdOutfit.SetActive(false);
    }
    #endregion Nerd Outfit
}
