using UnityEngine;
using System.Collections;

public class SetActiveEffect : Effect
{
    // Variables to specify in the editor.
    public GameObject objectToSetActive;
    [SerializeField] private bool activeAtStart = true;
    [SerializeField] private bool activeAtEnd = true;

    protected override void Awake() {
        base.Awake();
    }

    protected override void Start() {
        base.Start();
    }

    protected override void Update() {
        base.Update();
    }

    public override void TriggerEffect() {
        base.TriggerEffect();
    }

    public override void StartEffect() {
        base.StartEffect();

        if (objectToSetActive) {
            objectToSetActive.SetActive(activeAtStart);
        }
    }

    public override void EndEffect() {
        base.EndEffect();

        if (objectToSetActive) {
            objectToSetActive.SetActive(activeAtEnd);
        }
    }

    public void OnDrawGizmos() {
        if (objectToSetActive) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, objectToSetActive.transform.position);
        }
    }
}
