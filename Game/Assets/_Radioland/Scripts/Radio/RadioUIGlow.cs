using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadioUIGlow : MonoBehaviour
{
    private enum StationChoice {
        Station_1,
        Station_2
    }

    [SerializeField] private StationChoice stationChoice = StationChoice.Station_1;

    private RadioStation radioStation;
    private RadioStation[] allStations;
    private RectTransform rectTransform;

    private static float leftCenterX = 0.082f;
    private static float rightCenterX = 0.92f;
    private static float width = 0.12f;

    private void Awake() {
        GameObject player = GameObject.FindWithTag("Player");
        allStations = player.GetComponentsInChildren<RadioStation>();

        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void Start() {
        foreach (RadioStation station in allStations) {
            if ((station.id == 1 && stationChoice == StationChoice.Station_1) ||
                (station.id == 2 && stationChoice == StationChoice.Station_2)) {
                radioStation = station;
            }
        }

        float centerX = Mathf.Lerp(leftCenterX, rightCenterX, radioStation.frequency);

        Vector2 anchorMin = rectTransform.anchorMin;
        Vector2 anchorMax = rectTransform.anchorMax;
        anchorMin.x = centerX - width / 2f;
        anchorMax.x = centerX + width / 2f;
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }

    private void Update() {

    }
}
