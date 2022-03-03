using UnityEngine;

public class TestAction : Action
{
    [SerializeField] private float timeToTest = 30.0f;
    [SerializeField] private float timeToTriggerChanceCard = 5.0f;
    public void ModifyTestTime(float amount) { timeToTest += amount; }
    public void EndTest() { timeToTest = 0; }

    private Clone clone;
    private Renderer cloneRenderer;

    private float testTimer = 0.0f;
    private bool testing = false;

    private bool popupTriggered = false;

    protected override void Awake()
    {
        base.Awake();

        clone = FindObjectOfType<Clone>();
        cloneRenderer = clone.GetComponentInChildren<Renderer>();
    }

    public override bool StartAction()
    {
        actionStatus = Action_Status.STARTED;
        return true;
    }
    public override bool ContinueAction()
    {
        // When the clone reaches the entrance they start testing
        if (!testing && clone.ReachedDestination()) testing = true;
        // The testing timer runs while they're gone
        else if (testing)
        {
            cloneRenderer.enabled = false;
            testTimer += Time.deltaTime;

            // After a moment of the clone being gone we trigger the test
            if (testTimer >= timeToTriggerChanceCard && !popupTriggered)
            {
                ManagerHandler.instance.PopupM.ShowChanceCard();
                popupTriggered = true;
            }

            if (testTimer > timeToTest) actionStatus = Action_Status.COMPLETED;
        }
        return true;
    }
    public override void EndAction()
    {
        base.EndAction();
        cloneRenderer.enabled = true;
    }
    public override void CancelAction()
    {
        base.CancelAction();
    }
}
