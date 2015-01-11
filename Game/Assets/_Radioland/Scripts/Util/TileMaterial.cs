﻿using System.Linq;
using UnityEngine;

internal enum Axis
{
    WorldX, WorldY, WorldZ, LocalX, LocalY, LocalZ
}

public class TileMaterial : MonoBehaviour
{
    [SerializeField] private float xScaleMultipler = 1.0f;
    [SerializeField] private float yScaleMultipler = 1.0f;
    [SerializeField] [Tooltip("Scale with object scale")] private bool proportionalScale = true;
    [SerializeField] private Axis materialXAxis = Axis.WorldX;
    [SerializeField] private Axis materialYAxis = Axis.WorldY;
    [SerializeField] [Range(0.0f, 1.0f)] private float xOffset = 0.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float yOffset = 0.0f;
    [SerializeField] private string[] materialPropertyNames = {
        "_MainTex", "_NormalMap", "_SpecMap", "_BumpMap" };

    private void Awake() {
        float materialScaleX = xScaleMultipler * (proportionalScale ? GetScale(materialXAxis) : 1f);
        float materialScaleY = yScaleMultipler * (proportionalScale ? GetScale(materialYAxis) : 1f);

        foreach (string propertyName in materialPropertyNames.Where(propertyName => renderer.material.HasProperty(propertyName))) {
            renderer.material.SetTextureScale(propertyName, new Vector2(materialScaleX, materialScaleY));
            renderer.material.SetTextureOffset(propertyName, new Vector2(xOffset, yOffset));
        }
    }

    private float GetScale(Axis axis) {
        switch (axis) {
            case Axis.WorldX:
                return transform.lossyScale.x;
            case Axis.WorldY:
                return transform.lossyScale.y;
            case Axis.WorldZ:
                return transform.lossyScale.z;
            case Axis.LocalX:
                return transform.localScale.x;
            case Axis.LocalY:
                return transform.localScale.y;
            case Axis.LocalZ:
                return transform.localScale.z;
            default:
                return 1f;
        }
    }
}
