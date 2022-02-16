using UnityEngine;

public abstract class DirectorActionData : ScriptableObject
{
    public float[] insistencyChanges;

    public abstract void TriggerAction();
}