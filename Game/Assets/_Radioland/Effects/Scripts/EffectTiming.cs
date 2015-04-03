using System;
using UnityEngine;

[Serializable]
public class EffectTiming
{
    public float startDelay = 0.0f;
    public float cooldown = 0.0f;
    public bool timed = false;
    public float duration = 2.0f;
    public bool random = false;
    public float playChance = 1.0f;
}
