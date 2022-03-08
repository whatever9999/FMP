using UnityEngine;

public class PassOutAction : AnimationAction
{
    [SerializeField] private int passOutLength = 60;
    [SerializeField] private float sleepModifier = 2.0f;
    [SerializeField] private float timeToCheckSleepEffect = 1.0f;

    private int startPassedOut;
    private float sleepEffectTimer;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool StartAction()
    {
        base.StartAction();

        startPassedOut = ManagerHandler.instance.TimeM.GetCurrentTime();

        actionStatus = Action_Status.STARTED;
        return true;
    }
    public override bool ContinueAction()
    {
        int timePassedOut = ManagerHandler.instance.TimeM.TimeSince(startPassedOut);

        // Increase sleep while passed out
        sleepEffectTimer += Time.deltaTime;
        if (sleepEffectTimer >= timeToCheckSleepEffect)
        {
            ManagerHandler.instance.NeedsM.ModifyNeed(NeedsManager.NeedType.SLEEP, sleepModifier);
            sleepEffectTimer = 0.0f;
        }

        // After a moment of the clone being gone we trigger the test
        if (timePassedOut >= passOutLength)
        {
            actionStatus = Action_Status.COMPLETED;
        }
        return true;
    }
    public override void EndAction()
    {
        base.EndAction();
    }
    public override void CancelAction()
    {
        base.CancelAction();
    }
}
