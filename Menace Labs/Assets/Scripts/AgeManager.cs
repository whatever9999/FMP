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
        UIManager.instance.SetAgeTooltip(cloneAge);

        CheckAgeTrigger();
    }

    // If the clone has no days left the old age death will occur
    private void CheckAgeTrigger()
    {
        if (cloneAge <= 0)
        {
            EventManager.instance.CheckEventTrigger(EventManager.EventType.NUM_EVENT_TYPES);
        }
    }
}
