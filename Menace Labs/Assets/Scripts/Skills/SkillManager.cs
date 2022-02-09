using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public const int MAX_SKILL_LEVEL = 10;

    public enum SkillType
    {
         CLEANING,
         COOKING,
         DANCING,
         GAMING,
         HANDINESS,
         PHOTOGRAPHY,
         PROGRAMMING,
         DARTS,
         NONE,
    }

    [SerializeField] private int[] notchSizes;
    [SerializeField] private float[] notchMultipliers;
    public float GetNotchSize(int notch)
    {
        return notchSizes[notch];
    }
    public float GetNotchMultiplier(int notch)
    {
        return notchMultipliers[notch];
    }

    [SerializeField] private Skill[] skills;

    public void ProgressSkill(SkillType skillType, float amount)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].GetSkillType() == skillType)
            {
                skills[i].ProgressSkill(amount);
            }
        }
    }
    public void ModifySkill(SkillType skillType, int notches)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].GetSkillType() == skillType)
            {
                skills[i].ModifySkill(notches);
            }
        }
    }

    public int GetSkillLevel(SkillType skillType)
    {
        for(int i = 0; i < skills.Length; i++)
        {
            if(skills[i].GetSkillType() == skillType)
            {
                return skills[i].GetSkillLevel();
            }
        }

        Debug.LogError("Couldn't find skill of type " + skillType);
        return 0;
    }
}