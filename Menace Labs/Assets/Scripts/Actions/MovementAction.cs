using UnityEngine.AI;

public class MovementAction : Action
{
    NavMeshAgent clone;

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