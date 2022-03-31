using UnityEngine;

public class ManagerHandler : MonoBehaviour
{
    public static ManagerHandler instance;

    public CameraHandler camera;

    public Director director;
    public MenaceMetric MenaceMetric;
    public PerformanceMetric PerformanceMetric;

    public Clone clone;

    public ThoughtTooltip thoughtTooltip;

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
    public NotificationManager NotificationM;

    private void Awake()
    {
        instance = this;
    }
}
