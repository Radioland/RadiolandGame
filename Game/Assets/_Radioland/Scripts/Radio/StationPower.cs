using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class StationPower : MonoBehaviour
{
    [SerializeField] private RadioStation station;
    [SerializeField] [Tooltip("Enable when powered, disable without power.")]
    private List<MonoBehaviour> powerBehaviors;
    [SerializeField] private UnityEvent startPowerEvents;
    [SerializeField] private UnityEvent stopPowerEvents;

    private bool powered;

    private void Awake() {
        if (!station) {
            Debug.LogWarning("No RadioStation linked to " + this.GetPath());
            this.enabled = false;
        }

        StopPower();
    }

    private void Start() {

    }

    private void Update() {
        if (!powered && station.StrongSignal()) {
            StartPower();
        }

        if (powered && !station.StrongSignal()) {
            StopPower();
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
