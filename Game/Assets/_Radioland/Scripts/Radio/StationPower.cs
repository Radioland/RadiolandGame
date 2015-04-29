using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StationPower : MonoBehaviour
{
    public enum StationChoice {
        Any,
        Station_1,
        Station_2,
        Station_3
    }

    public StationChoice stationChoice = StationChoice.Any;
    [SerializeField] private bool stopOnAwake = true;
    [SerializeField] [Tooltip("Enable when powered, disable without power.")]
    private List<MonoBehaviour> powerBehaviors;
    [SerializeField] private UnityEvent unlockEvents;
    [SerializeField] private UnityEvent startPowerEvents;
    [SerializeField] private UnityEvent stopPowerEvents;
    [SerializeField] private UnityEventFloat continuousEvents;

    private bool powered;
    private RadioControl radioControl;
    private RadioStation radioStation;
    private RadioStation[] allStations;

    private void Awake() {
        GameObject player = GameObject.FindWithTag("Player");
        radioControl = player.GetComponentInChildren<RadioControl>();
        allStations = player.GetComponentsInChildren<RadioStation>();
    }

    private void Start() {
        // Find the radioStation matching stationChoice.
        foreach (RadioStation station in allStations) {
            if ((station.id == 1 && stationChoice == StationChoice.Station_1) ||
                (station.id == 2 && stationChoice == StationChoice.Station_2) ||
                (station.id == 3 && stationChoice == StationChoice.Station_3)) {
                radioStation = station;
            }
        }

        Messenger.AddListener<int>("UnlockStation", OnUnlockStation);

        if (enabled && stopOnAwake) {
            StopPower(stopEvenIfAlreadyStopped:true);
        }
    }

    private void Update() {
        if (!enabled) { return; }

        if (stationChoice == StationChoice.Any) {
            if (radioControl.anyStrongSignal) {
                StartPower();
            } else {
                StopPower();
            }

            continuousEvents.Invoke(radioControl.strongestSignal);
        } else if (radioStation) {
            if (radioStation.StrongSignal()) {
                StartPower();
            } else {
                StopPower();
            }

            continuousEvents.Invoke(radioStation.signalStrength);
        }
    }

    private void OnUnlockStation(int stationId) {
        if ((stationChoice == StationChoice.Station_1 && stationId == 1) ||
            (stationChoice == StationChoice.Station_2 && stationId == 2) ||
            (stationChoice == StationChoice.Station_3 && stationId == 3) ||
            (stationChoice == StationChoice.Any)) {
            unlockEvents.Invoke();
        }
    }

    private void StartPower(bool startEvenIfAlreadyStarted=false) {
        if (powered && !startEvenIfAlreadyStarted) { return; }

        powered = true;
        foreach (MonoBehaviour powerBehavior in powerBehaviors) {
            powerBehavior.enabled = true;
        }
        startPowerEvents.Invoke();
    }

    private void StopPower(bool stopEvenIfAlreadyStopped=false) {
        if (!powered && !stopEvenIfAlreadyStopped) { return; }

        powered = false;
        foreach (MonoBehaviour powerBehavior in powerBehaviors) {
            powerBehavior.enabled = false;
        }
        stopPowerEvents.Invoke();
        continuousEvents.Invoke(0);
    }
}
