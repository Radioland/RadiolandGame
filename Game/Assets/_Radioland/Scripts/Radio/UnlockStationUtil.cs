using UnityEngine;
using System.Collections;

public class UnlockStationUtil : MonoBehaviour
{
    private enum StationChoice {
        Station_1,
        Station_2,
        Station_3
    }

    [SerializeField] private StationChoice stationChoice;
    [SerializeField] private UnlockStationEffect unlockStation;
    [SerializeField] private StationPower stationPower;

    private void Reset() {
        unlockStation = gameObject.GetComponentInChildren<UnlockStationEffect>();
        stationPower = gameObject.GetComponentInChildren<StationPower>();
    }

    private void Awake() {
        switch (stationChoice) {
            case StationChoice.Station_1:
                stationPower.stationChoice = StationPower.StationChoice.Station_1;
                unlockStation.stationId = 1;
                break;
            case StationChoice.Station_2:
                stationPower.stationChoice = StationPower.StationChoice.Station_2;
                unlockStation.stationId = 2;
                break;
            case StationChoice.Station_3:
                stationPower.stationChoice = StationPower.StationChoice.Station_3;
                unlockStation.stationId = 3;
                break;
            default:
                break;
        }
    }

    private void Start() {

    }

    private void Update() {

    }
}
