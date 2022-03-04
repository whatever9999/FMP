using UnityEngine;

public abstract class DirectorActionData : ScriptableObject
{
    [TextArea(10, 30)]
    public string insistencyChangesInfo = "Corresponds to DirectorActionType enum: \n0\tDECREASE_NEED_HUNGER, " +
        "\n1\tDECREASE_NEED_FUN, \n2\tDECREASE_NEED_COMFORT, \n3\tDECREASE_NEED_SOCIAL, \n4\tDECREASE_NEED_BLADDER," +
        "\n5\tDECREASE_NEED_HYGIENE,  \n6\tDECREASE_NEED_SLEEP, \n7\tDECREASE_NEED_ENVIRONMENT, " +
        "\n8\tDECREASE_SKILL_CLEANING, \n9\tDECREASE_SKILL_HANDINESS, \n10\tDECREASE_SKILL_COOKING," +
        "\n11\tDECREASE_SKILL_PHOTOGRAPHY, \n12\tDECREASE_SKILL_DANCING, \n13\tDECREASE_SKILL_PROGRAMMING, " +
        "\n14\tDECREASE_SKILL_GAMING, \n15\tDECREASE_SKILL_DARTS, \n16\tINCREASE_MADNESS_CHANCE";
    [Range(-1, 1)]
    public float[] insistencyChanges;

    public abstract void TriggerAction();
}