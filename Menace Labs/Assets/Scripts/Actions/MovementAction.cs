using UnityEngine;
using UnityEngine.AI;

public class MovementAction : Action
{
    [SerializeField] private float stoppingDistance = 0.5f;

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
        completed = ReachedDestination();
        return true;
    }
    public override void EndAction()
    {
        clone.SetDestination(clone.transform.position);
    }
    public override void CancelAction()
    {
        clone.SetDestination(clone.transform.position);
    }

    private bool ReachedDestination()
    {
        // If the clone is within stopping distance then they've reached their destination
        if (clone.transform.position.x > clone.destination.x - stoppingDistance &&
            clone.transform.position.x < clone.destination.x + stoppingDistance &&
            clone.transform.position.z > clone.destination.z - stoppingDistance &&
            clone.transform.position.z < clone.destination.z + stoppingDistance)
        {
            return true;
        }
        return false;
    }
}