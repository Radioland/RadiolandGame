using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StationPower : MonoBehaviour
{
    private enum StationChoice {
        Any,
        Station_1,
        Station_2,
        Station_3
    }

    [SerializeField] private StationChoice stationChoice = StationChoice.Any;
    [SerializeField] [Tooltip("Enable when powered, disable without power.")]
    private List<MonoBehaviour> powerBehaviors;
    [SerializeField] private UnityEvent startPowerEvents;
    [SerializeField] private UnityEvent stopPowerEvents;

    private bool powered;
    private RadioStation radioStation;
    private RadioStation[] allStations;

    private void Awake() {
        GameObject player = GameObject.FindWithTag("Player");
        allStations = player.GetComponentsInChildren<RadioStation>();

        // Find the radioStation matching stationChoice.
        foreach (RadioStation station in allStations) {
            if ((station.id == 1 && stationChoice == StationChoice.Station_1) ||
                (station.id == 2 && stationChoice == StationChoice.Station_2) ||
                (station.id == 3 && stationChoice == StationChoice.Station_3)) {
                radioStation = station;
            }
        }

        StopPower();
    }

    private void Start() {

    }

    private void Update() {
        if (stationChoice == StationChoice.Any) {
            if (allStations.Any(station => station.StrongSignal())) {
                StartPower();
            } else {
                StopPower();
            }
        } else if (radioStation) {
            if (!powered && radioStation.StrongSignal()) {
                StartPower();
            }

            if (powered && !radioStation.StrongSignal()) {
                StopPower();
            }
        }
    }

    private void StartPower() {
        powered = true;
        foreach (MonoBehaviour powerBehavior in powerBehaviors) {
            powerBehavior.enabled = true;
        }
        startPowerEvents.Invoke();
    }

    private void StopPower() {
        powered = false;
        foreach (MonoBehaviour powerBehavior in powerBehaviors) {
            powerBehavior.enabled = false;
        }
        stopPowerEvents.Invoke();
    }
}
