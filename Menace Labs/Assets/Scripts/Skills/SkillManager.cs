using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

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
         SNOOKER,
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

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        ProgressSkill(SkillType.DANCING, 1);
    }

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