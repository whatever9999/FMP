using UnityEngine;
using UnityEngine.AI;

public class MovementAction : Action
{
    private NavMeshAgent clone;

    private Vector3 destination;
    public void SetDestination(Vector3 newDestination) { destination = newDestination; }

    public override bool StartAction()
    {
        clone = FindObjectOfType<NavMeshAgent>();
        clone.SetDestination(destination);
        started = true;
        return true;
    }
    public override bool ContinueAction()
    {
        completed = clone.transform.position == clone.destination;
        return true;
    }
    public override void EndAction()
    {

    }
    public override void CancelAction()
    {
        clone.SetDestination(clone.transform.position);
    }
}