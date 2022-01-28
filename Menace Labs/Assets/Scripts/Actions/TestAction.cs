using UnityEngine;

public class TestAction : Action
{
    [SerializeField] private float timeToTest = 10.0f;

    private Clone clone;

    private float testTimer = 0.0f;
    private bool testing = false;

    private void Awake()
    {
        clone = FindObjectOfType<Clone>();
    }

    public override bool StartAction()
    {
        ActionManager.instance.AddAction(ActionManager.ActionType.MOVE_TO_ENTRANCE, true);
        started = true;
        return true;
    }
    public override bool ContinueAction()
    {
        // When the clone reaches the entrance they start testing
        if (!testing && clone.ReachedDestination()) testing = true;
        // The testing timer runs while they're gone
        else if (testing)
        {
            clone.gameObject.SetActive(false);
            testTimer += Time.deltaTime;
            completed = testTimer > timeToTest;
        }
        return true;
    }
    public override void EndAction()
    {
        clone.gameObject.SetActive(true);
    }
    public override void CancelAction()
    {

    }
}
