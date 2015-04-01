using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MessageEffect : Effect
{
    public enum MessageFormat
    {
        None, Int, Float, String, Bool
    }

    [SerializeField] private String messageName;
    public MessageFormat format = MessageFormat.None;

    // TODO: flags for when to call this? (Trigger, Start, End)
    //       or expand on the custom inspector to provide options for all

    [SerializeField] [HideInInspector] private int messageValueInt;
    [SerializeField] [HideInInspector] private float messageValueFloat;
    [SerializeField] [HideInInspector] private string messageValueString;
    [SerializeField] [HideInInspector] private bool messageValueBool;

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

        switch (format) {
            case MessageFormat.None:
                Messenger.Broadcast(messageName);
                break;
            case MessageFormat.Int:
                Messenger.Broadcast(messageName, messageValueInt);
                break;
            case MessageFormat.Float:
                Messenger.Broadcast(messageName, messageValueFloat);
                break;
            case MessageFormat.String:
                Messenger.Broadcast(messageName, messageValueString);
                break;
            case MessageFormat.Bool:
                Messenger.Broadcast(messageName, messageValueBool);
                break;
            default:
                break;
        }
    }

    public override void EndEffect() {
        base.EndEffect();
    }
}
