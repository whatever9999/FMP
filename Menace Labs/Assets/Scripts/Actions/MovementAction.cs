using UnityEngine;
using UnityEngine.AI;

public class MovementAction : Action
{
    private Clone clone;
    private NavMeshAgent cloneMovement;

    private Vector3 destination;
    public void SetDestination(Vector3 newDestination) { destination = newDestination; }

    protected override void Awake()
    {
        base.Awake();

        clone = FindObjectOfType<Clone>();
        cloneMovement = clone.GetComponent<NavMeshAgent>();
    }

    public override bool StartAction()
    {
        cloneMovement.SetDestination(destination);
        actionStatus = Action_Status.STARTED;
        return true;
    }
    public override bool ContinueAction()
    {
        if (clone.ReachedDestination()) actionStatus = Action_Status.COMPLETED;
        return true;
    }
    public override void EndAction()
    {
        base.EndAction();
        cloneMovement.SetDestination(clone.transform.position);
    }
    public override void CancelAction()
    {
        base.CancelAction();
        // Only change the destination if this is the current action
        if (ManagerHandler.instance.ActionM.GetCurrentAction() == this)
        {
            cloneMovement.SetDestination(clone.transform.position);
        }
    }

    
}