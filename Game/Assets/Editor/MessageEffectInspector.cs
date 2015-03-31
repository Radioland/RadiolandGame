using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MessageEffect))]
public class MessageEffectInspector : Editor
{
    private MessageEffect messageEffect;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        messageEffect = target as MessageEffect;
        SerializedObject so = new SerializedObject(messageEffect);

        GUIContent label = new GUIContent("Message Value");

        switch (messageEffect.format) {
            case MessageEffect.MessageFormat.None:
                break;
            case MessageEffect.MessageFormat.Int:
                EditorGUILayout.PropertyField(so.FindProperty("messageValueInt"), label);
                break;
            case MessageEffect.MessageFormat.Float:
                EditorGUILayout.PropertyField(so.FindProperty("messageValueFloat"), label);
                break;
            case MessageEffect.MessageFormat.String:
                EditorGUILayout.PropertyField(so.FindProperty("messageValueString"), label);
                break;
            case MessageEffect.MessageFormat.Bool:
                EditorGUILayout.PropertyField(so.FindProperty("messageValueBool"), label);
                break;
            default:
                break;
        }

        so.ApplyModifiedProperties();
    }
}
