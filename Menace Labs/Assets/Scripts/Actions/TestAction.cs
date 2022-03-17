using UnityEngine;

public class TestAction : Action
{
    [SerializeField] private float timeToTest = 30.0f;
    [SerializeField] private float timeToTriggerChanceCard = 5.0f;
    public void ModifyTestTime(float amount) { timeToTest += amount; }
    public void EndTest() { timeToTest = 0; }

    private Clone clone;
    

    private int startTestTime;
    private bool testing = false;

    private bool popupTriggered = false;

    protected override void Awake()
    {
        base.Awake();

        clone = FindObjectOfType<Clone>();

        ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.TEST);
    }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;
        return true;
    }
    public override bool ContinueAction()
    {
        // When the clone reaches the entrance they start testing
        if (!testing && clone.ReachedDestination())
        {
            testing = true;
            // Store the time the test started so we can identify when to stop the test
            startTestTime = ManagerHandler.instance.TimeM.GetCurrentTime();
        }
        // The testing timer runs while they're gone
        else if (testing)
        {
            clone.ToggleClone(false);
            int timeInTest = ManagerHandler.instance.TimeM.TimeSince(startTestTime);

            // After a moment of the clone being gone we trigger the test
            if (timeInTest >= timeToTriggerChanceCard && !popupTriggered)
            {
                ManagerHandler.instance.PopupM.ShowChanceCard();
                popupTriggered = true;
            }

            if (timeInTest > timeToTest) actionStatus = Action_Status.COMPLETED;
        }
        return true;
    }
    public override void EndAction()
    {
        base.EndAction();
        clone.ToggleClone(true);
        ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.TEST_FINISHED);
    }
    public override void CancelAction()
    {
        base.CancelAction();
        clone.ToggleClone(true);
        ManagerHandler.instance.NotificationM.AddNotification(NotificationManager.NotificationType.TEST_FINISHED);
    }
}
