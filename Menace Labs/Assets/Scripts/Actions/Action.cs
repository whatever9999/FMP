using UnityEngine;

public abstract class Action : MonoBehaviour
{
    [SerializeField] private bool cancellable;

    protected bool completed;
    protected bool started;
    public bool HasStarted() { return started; }
    public bool HasCompleted() { return completed; }

    public abstract bool StartAction();
    public abstract bool ContinueAction();
    public abstract void EndAction();
    public abstract void CancelAction();

    public void OnMouseUp()
    {
        if (cancellable)
        {
            ActionManager.instance.CancelAction(gameObject);
        }
    }
}