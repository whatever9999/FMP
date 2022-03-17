using UnityEngine;

public class PassOutAction : AnimationAction
{
    [SerializeField] private int passOutLength = 60;

    private int startPassedOut;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool StartAction()
    {
        base.StartAction();

        startPassedOut = ManagerHandler.instance.TimeM.GetCurrentTime();
        ManagerHandler.instance.NeedsM.SetPassedOut(true);

        actionStatus = Action_Status.STARTED;
        return true;
    }
    public override bool ContinueAction()
    {
        int timePassedOut = ManagerHandler.instance.TimeM.TimeSince(startPassedOut);

        // After a moment of the clone being gone we trigger the test
        if (timePassedOut >= passOutLength)
        {
            actionStatus = Action_Status.COMPLETED;
        }
        return true;
    }
    public override void EndAction()
    {
        ManagerHandler.instance.NeedsM.SetPassedOut(false);
        base.EndAction();
    }
    public override void CancelAction()
    {
        ManagerHandler.instance.NeedsM.SetPassedOut(false);
        base.CancelAction();
    }
}
