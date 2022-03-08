using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DeathData")]
public class DeathData : ScriptableObject
{
    public enum DeathTypes
    {
        FIRE,
        OLD_AGE,
        ELECTROCUTION,
        STARVATION,
        MADNESS,
        NUM_DEATH_TYPES,
    }

    public DeathTypes type;
    [TextArea]
    public string description;
}
