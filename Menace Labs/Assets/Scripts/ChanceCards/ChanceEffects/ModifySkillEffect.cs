using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/ModifySkill")]
public class ModifySkillEffect : ChanceEffect
{
    public SkillManager.SkillType skillType;
    public int notches;

    public override void TriggerEffect()
    {
        ManagerHandler.instance.SkillM.ModifySkill(skillType, notches);
    }
}