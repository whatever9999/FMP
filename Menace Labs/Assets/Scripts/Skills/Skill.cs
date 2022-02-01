using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    private Slider skillSlider;

    private SkillManager.SkillType skillType;
    private int currentSkillLevel;
    private float currentNotchProgress;

    public SkillManager.SkillType GetSkillType() { return skillType; }
    public int GetSkillLevel() { return currentSkillLevel; }

    private void Start()
    {
        skillSlider = GetComponent<Slider>();
    }

    public void ProgressSkill(int amount)
    {
        // The current notch increases by amount * skill level multiplier
        currentNotchProgress += amount * SkillManager.instance.GetNotchMultiplier(currentSkillLevel);

        // If we've gone above the size of the current level then go up a level and reduce our current progress
        if (currentNotchProgress >= SkillManager.instance.GetNotchSize(currentSkillLevel))
        {
            currentNotchProgress -= SkillManager.instance.GetNotchSize(++currentSkillLevel);
            // Update the UI
            skillSlider.value = currentSkillLevel;
        }
    }
    public void ModifySkill(int notches)
    {
        currentSkillLevel += notches;
        skillSlider.value = currentSkillLevel;
    }
}
