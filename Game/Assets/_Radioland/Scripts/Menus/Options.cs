using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : MonoBehaviour
{
    [SerializeField] private Text invertXText;
    [SerializeField] private Text invertYText;
    [SerializeField] private Text muteText;

    private void Awake() {
        SetInvertXText();
        SetInvertYText();
    }

    private void Start() {

    }

    private void Update() {

    }

    public void ToggleInvertX() {
        int currentValue = PlayerPrefs.GetInt("InvertX", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("InvertX", newValue);

        SetInvertXText();

        Messenger.Broadcast("OptionsChanged");
    }

    public void ToggleInvertY() {
        int currentValue = PlayerPrefs.GetInt("InvertY", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("InvertY", newValue);

        SetInvertYText();

        Messenger.Broadcast("OptionsChanged");
    }

    public void ToggleMute() {
        int currentValue = PlayerPrefs.GetInt("Mute", 0);
        int newValue = currentValue == 0 ? 1 : 0;
        PlayerPrefs.SetInt("Mute", newValue);

        SetMuteText();

        Messenger.Broadcast("OptionsChanged");
    }

    private void SetInvertXText() {
        int currentValue = PlayerPrefs.GetInt("InvertX", 0);
        invertXText.text = currentValue == 0 ? "Off" : "On";
    }

    private void SetInvertYText() {
        int currentValue = PlayerPrefs.GetInt("InvertY", 0);
        invertYText.text = currentValue == 0 ? "Off" : "On";
    }

    private void SetMuteText() {
        int currentValue = PlayerPrefs.GetInt("Mute", 0);
        muteText.text = currentValue == 0 ? "Off" : "On";
    }
}
