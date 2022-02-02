using UnityEngine;

public class ManagerHandler : MonoBehaviour
{
    public static ManagerHandler instance;

    public Clone clone;
    public ActionManager ActionM;
    public AgeManager AgeM;
    public AnimationManager AnimationM;
    public EventManager EventM;
    public FoodManager FoodM;
    public GoalManager GoalM;
    public NeedsManager NeedsM;
    public PopUpManager PopupM;
    public SkillManager SkillM;
    public SoundManager SoundM;
    public TimeManager TimeM;
    public UIManager UIM;

    private void Awake()
    {
        instance = this;
    }
}
