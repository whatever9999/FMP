using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private List<ObjectEffect> effects;
}

[System.Serializable]
public struct ObjectEffect
{
    public enum ObjectEffectType
    {
        HUNGER,
        COMFORT,
        BLADDER,
        SLEEP,
        FUN,
        SOCIAL,
        HYGIENE,
        ENVIRONMENT,
        CLEANING,
        COOKING,
        DANCING,
        GAMING,
        HANDINESS,
        PHOTOGRAPHY,
        PROGRAMMING,
        SNOOKER,
    }

    [SerializeField] private ObjectEffectType type;
    [SerializeField] private float value;
    public ObjectEffectType GetEffectType() { return type; }
    public float GetValue() { return value; }
}