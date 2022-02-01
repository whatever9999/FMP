using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    [SerializeField] private SkillManager.SkillType skillType;

    private Slider skillSlider;

    private int currentSkillLevel;
    private float currentNotchProgress;

    public SkillManager.SkillType GetSkillType() { return skillType; }
    public int GetSkillLevel() { return currentSkillLevel; }

    private void Start()
    {
        skillSlider = GetComponent<Slider>();
    }

    public void ProgressSkill(float amount)
    {
        if (currentSkillLevel < SkillManager.MAX_SKILL_LEVEL)
        {
            // The current notch increases by amount * skill level multiplier
            currentNotchProgress += amount * SkillManager.instance.GetNotchMultiplier(currentSkillLevel);

            // If we've gone above the size of the current level then go up a level and reduce our current progress
            if (currentNotchProgress >= SkillManager.instance.GetNotchSize(currentSkillLevel))
            {
                // If this is the last skill point we won't make any more progress on it so just increase the skill and update the UI
                if (currentSkillLevel == SkillManager.MAX_SKILL_LEVEL - 1)
                {
                    currentSkillLevel++;
                    currentNotchProgress = 0;
                    skillSlider.value = currentSkillLevel;
                }
                else
                {
                    currentNotchProgress -= SkillManager.instance.GetNotchSize(++currentSkillLevel);
                    // Update the UI
                    skillSlider.value = currentSkillLevel;
                }
            }
        }
    }
    public void ModifySkill(int notches)
    {
        currentSkillLevel += notches;
        skillSlider.value = currentSkillLevel;
    }
}
