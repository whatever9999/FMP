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
        if (cloneAge <= 0)
        {
            ManagerHandler.instance.EventM.SetDeathEventType(DeathData.DeathTypes.OLD_AGE);
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.DIE, 1);
        }
    }
}
