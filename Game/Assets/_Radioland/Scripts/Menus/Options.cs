using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : MonoBehaviour
{
    [Header("Toggle Images")]
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [Header("Fields")]
    [SerializeField] private Text invertXText;
    [SerializeField] private Text invertYText;
    [SerializeField] private Text muteText;
    [SerializeField] private Image invertXImage;
    [SerializeField] private Image invertYImage;
    [SerializeField] private Image muteImage;

    private void Awake() {
        SetInvertX();
        SetInvertY();
    }

    private void Start() {

    }

    private void Update() {

    }

    public void ToggleInvertX() {
        int currentValue = PlayerPrefs.GetInt("InvertX", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("InvertX", newValue);

        SetInvertX();

        Messenger.Broadcast("OptionsChanged");
    }

    public void ToggleInvertY() {
        int currentValue = PlayerPrefs.GetInt("InvertY", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("InvertY", newValue);

        SetInvertY();

        Messenger.Broadcast("OptionsChanged");
    }

    public void ToggleMute() {
        int currentValue = PlayerPrefs.GetInt("Mute", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("Mute", newValue);

        SetMute();

        Messenger.Broadcast("OptionsChanged");
    }

    private void SetInvertX() {
        int currentValue = PlayerPrefs.GetInt("InvertX", 0);
        invertXText.text = currentValue == 0 ? "Off" : "On";
        invertXImage.sprite = currentValue == 0 ? offSprite : onSprite;
    }

    private void SetInvertY() {
        int currentValue = PlayerPrefs.GetInt("InvertY", 0);
        invertYText.text = currentValue == 0 ? "Off" : "On";
        invertYImage.sprite = currentValue == 0 ? offSprite : onSprite;
    }

    private void SetMute() {
        int currentValue = PlayerPrefs.GetInt("Mute", 0);
        muteText.text = currentValue == 0 ? "Off" : "On";
        muteImage.sprite = currentValue == 0 ? offSprite : onSprite;
    }
}
