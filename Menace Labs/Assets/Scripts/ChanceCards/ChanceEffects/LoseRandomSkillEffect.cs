using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ChanceEffects/LoseRandomSkill")]
public class LoseRandomSkillEffect : ChanceEffect
{
    public int notches;

    public override void TriggerEffect()
    {
        SkillManager.SkillType skillType = (SkillManager.SkillType)Random.Range(0, (int)SkillManager.SkillType.NONE);

        ManagerHandler.instance.SkillM.ModifySkill(skillType, notches);
    }
}