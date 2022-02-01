using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public enum SkillType
    {
         CLEANING,
         COOKING,
         DANCING,
         GAMING,
         HANDINESS,
         PHOTOGRAPHY,
         PROGRAMMING,
         SNOOKER
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
        ProgressSkill(SkillType.CLEANING, 1);
    }

    public void ProgressSkill(SkillType skillType, int amount)
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