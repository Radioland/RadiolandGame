using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EffectTiming))]
public class EffectTimingDrawer : PropertyDrawer
{
    private bool foldout = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        float originalWidth = position.width;
        position.height = 16f;
        if (foldout) { position.width = 50f; }
        foldout = EditorGUI.Foldout(position, foldout, label);
        position.width = originalWidth;

        EditorGUI.BeginProperty(position, label, property);

        if (foldout) {
            position.height = GetPropertyHeight(property, label);
            position = EditorGUI.PrefixLabel(position, label);

            Rect startDelayPosition = new Rect(position.x, position.y, position.width, 16);
            Rect cooldownPosition = new Rect(position.x, position.y + 20, position.width, 16);
            Rect timedPosition = new Rect(position.x, position.y + 40, 60, 16);
            Rect durationPosition = new Rect(position.x + 60, position.y + 40, position.width - 60, 16);

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
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (foldout) {
            return 60f;
        } else {
            return base.GetPropertyHeight(property, label);
        }
    }
}
