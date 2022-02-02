using UnityEngine;
using UnityEngine.UI;

public class AgeManager : MonoBehaviour
{
    [SerializeField] private int cloneStartAge = 6;
    
    private Slider cloneAgeSlider;

    private int cloneAge;
    public int GetCloneAge() { return cloneAge; }

    void Start()
    {
        cloneAge = cloneStartAge;
        cloneAgeSlider = GetComponent<Slider>();

        ModifyCloneAge(0);
    }

    public void ModifyCloneAge(int days)
    {
        cloneAge += days;

        // Clamp
        if (cloneAge < 0) cloneAge = 0;
        if (cloneAge > cloneStartAge) cloneAge = cloneStartAge;

        cloneAgeSlider.value = cloneAge;
        ManagerHandler.instance.UIM.SetAgeTooltip(cloneAge);

        CheckAgeTrigger();
    }

    // If the clone has no days left the old age death will occur
    private void CheckAgeTrigger()
    {
        if (cloneAge <= 6)
        {
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.DIE, true);
        }
    }
}
