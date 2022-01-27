using UnityEngine;

public abstract class Action : MonoBehaviour
{
    [SerializeField] private bool completable;
    [SerializeField] private bool cancellable;

    private bool started;

    public abstract bool StartAction();
    public abstract bool ContinueAction();
    public abstract void EndAction();
    public abstract void CancelAction();
}

public class MovementAction : Action
{
    public override bool StartAction()
    {
        return true;
    }
    public override bool ContinueAction()
    {
        return true;
    }
    public override void EndAction()
    {

    }
    public override void CancelAction()
    {

    }
}