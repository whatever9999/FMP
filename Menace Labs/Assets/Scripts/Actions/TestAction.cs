using UnityEngine;

public class TestAction : Action
{
    [SerializeField] private float timeToTest = 10.0f;
    public void ModifyTestTime(float amount) { timeToTest += amount; }
    public void EndTest() { timeToTest = 0; }

    private Clone clone;
    private Renderer cloneRenderer;

    private float testTimer = 0.0f;
    private bool testing = false;

    private void Awake()
    {
        clone = FindObjectOfType<Clone>();
        cloneRenderer = clone.GetComponentInChildren<Renderer>();
    }

    public override bool StartAction()
    {
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
            cloneRenderer.enabled = false;
            testTimer += Time.deltaTime;
            completed = testTimer > timeToTest;
        }
        return true;
    }
    public override void EndAction()
    {
        cloneRenderer.enabled = true;
    }
    public override void CancelAction()
    {

    }
}
