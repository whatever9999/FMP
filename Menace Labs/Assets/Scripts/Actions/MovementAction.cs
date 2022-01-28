using UnityEngine;
using UnityEngine.AI;

public class MovementAction : Action
{
    private Clone clone;
    private NavMeshAgent cloneMovement;

    private Vector3 destination;
    public void SetDestination(Vector3 newDestination) { destination = newDestination; }

    private void Awake()
    {
        clone = FindObjectOfType<Clone>();
        cloneMovement = clone.GetComponent<NavMeshAgent>();
    }

    public override bool StartAction()
    {
        cloneMovement.SetDestination(destination);
        started = true;
        return true;
    }
    public override bool ContinueAction()
    {
        completed = clone.ReachedDestination();
        return true;
    }
    public override void EndAction()
    {
        cloneMovement.SetDestination(clone.transform.position);
    }
    public override void CancelAction()
    {
        // Only change the destination if this is the current action
        if (ActionManager.instance.GetCurrentAction() == this)
        {
            cloneMovement.SetDestination(clone.transform.position);
        }
    }

    
}