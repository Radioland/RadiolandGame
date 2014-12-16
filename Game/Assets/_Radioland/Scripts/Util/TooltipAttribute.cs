using UnityEngine;

// Adds a tooltip to a property, use like "[Tooltip("This is a great tooltip!")]".
// Works with Editor/TooltipDrawer.cs.
// Thanks to http://answers.unity3d.com/questions/37177/additional-information-on-mouseover-in-the-inspect.html

public class TooltipAttribute : PropertyAttribute
{
    public readonly string text;
    
    public TooltipAttribute(string text)
    {
        this.text = text;
    }
}