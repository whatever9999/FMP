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

    [SerializeField] private GameObject skillCapsule;
    [SerializeField] private Transform skillFill;
    private float skillFillStartYScale;
    public void ToggleSkillCapsule(bool enable) 
    {
        // Immediately disable but show when skill starts increasing so it has the correct scale
        showSkillCapsule = enable;
        if (!enable) skillCapsule.SetActive(false); 
    }
    private bool showSkillCapsule = false;

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
        skillFillStartYScale = skillFill.localScale.y;
    }

    public void ProgressSkill(SkillType skillType, float amount)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].GetSkillType() == skillType)
            {
                skills[i].ProgressSkill(amount);

                if (skills[i].GetSkillLevel() == SkillManager.MAX_SKILL_LEVEL)
                {
                    skillCapsule.SetActive(false);
                }
                else
                {
                    UpdateSkillCapsule(skills[i].GetNotchProgress(), skills[i].GetSkillLevel());
                    if (showSkillCapsule) skillCapsule.SetActive(true);
                }
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

    public void UpdateSkillCapsule(float progress, int level)
    {
        float notchPercentage = progress / notchSizes[level];

        Vector3 newScale = skillFill.localScale;
        newScale.y = skillFillStartYScale * notchPercentage;
        skillFill.localScale = newScale;
    }
}