using UnityEditor;
using UnityEngine;

// Adds a tooltip to a property, use like "[Tooltip("This is a great tooltip!")]".
// Works with Scripts/Util/TooltipAttribute.cs.
// Thanks to http://answers.unity3d.com/questions/37177/additional-information-on-mouseover-in-the-inspect.html

[CustomPropertyDrawer(typeof(TooltipAttribute))]
public class TooltipDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        var atr = (TooltipAttribute) attribute;
        var content = new GUIContent(label.text, atr.text);
        EditorGUI.PropertyField(position, prop, content);
    }
}