using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(EffectTiming))]
public class EffectTimingDrawer : PropertyDrawer
{
    private bool foldout = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        Rect originalPosition = position;
        if (foldout) { position.width = 50f; }
        foldout = EditorGUI.Foldout(position, foldout, label);
        position = originalPosition;
        position.height = GetPropertyHeight(property, label);

        EditorGUI.BeginProperty(position, label, property);
        // PrefixLabel separates the canvas into the label (left) and properties (right).
        // Pass a space as the string since this label is drawn above the foldout label.
        position = EditorGUI.PrefixLabel(position, new GUIContent(" "));
        if (foldout) {
            Rect startDelayPosition = new Rect(position.x, position.y, position.width, 16);
            Rect cooldownPosition = new Rect(position.x, position.y + 20, position.width, 16);
            Rect timedPosition = new Rect(position.x, position.y + 40, 60, 16);
            Rect durationPosition = new Rect(position.x + 60, position.y + 40, position.width - 60, 16);
            Rect randomPosition = new Rect(position.x, position.y + 60, position.width, 16);
            Rect playChancePosition = new Rect(position.x, position.y + 80, position.width, 16);

            EditorGUIUtility.labelWidth = 80;
            EditorGUI.PropertyField(startDelayPosition, property.FindPropertyRelative("startDelay"));
            EditorGUI.PropertyField(cooldownPosition, property.FindPropertyRelative("cooldown"));

            EditorGUIUtility.labelWidth = 42;
            EditorGUI.PropertyField(timedPosition, property.FindPropertyRelative("timed"),
                                    new GUIContent("Timed"));
            if (property.FindPropertyRelative("timed").boolValue) {
                EditorGUIUtility.labelWidth = 60;
                EditorGUI.PropertyField(durationPosition, property.FindPropertyRelative("duration"));
            }

            EditorGUIUtility.labelWidth = 80;
            EditorGUI.PropertyField(randomPosition, property.FindPropertyRelative("random"),
                                    new GUIContent("Random"));
            if (property.FindPropertyRelative("random").boolValue) {
                EditorGUI.Slider(playChancePosition, property.FindPropertyRelative("playChance"), 0f, 1f);
            }
        } else {
            // Display a preview of the settings on a single line.
            float startDelay = property.FindPropertyRelative("startDelay").floatValue;
            float cooldown = property.FindPropertyRelative("cooldown").floatValue;
            bool timed = property.FindPropertyRelative("timed").boolValue;
            float duration = property.FindPropertyRelative("duration").floatValue;
            bool random = property.FindPropertyRelative("random").boolValue;
            float playChance = property.FindPropertyRelative("playChance").floatValue;

            // Only show interesting (non-zero) values.
            List<string> messages = new List<string>();
            if (startDelay > 0.001) { messages.Add(startDelay + "s delay"); }
            if (cooldown > 0.001) { messages.Add(cooldown + "s cooldown"); }
            if (timed) { messages.Add(duration + "s duration"); }
            if (random) { messages.Add(playChance * 100 + "% chance"); }
            string message = string.Join(", ", messages.ToArray());
            EditorGUI.LabelField(position, message);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (foldout) {
            return 80f + (20 * (property.FindPropertyRelative("random").boolValue ? 1 : 0));
        } else {
            return base.GetPropertyHeight(property, label);
        }
    }
}
