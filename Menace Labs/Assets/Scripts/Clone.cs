using UnityEngine;
using UnityEngine.AI;

public class Clone : MonoBehaviour
{
    [SerializeField] Transform hand;

    private NavMeshAgent clone;

    private void Start()
    {
        clone = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ManagerHandler.instance.ActionM.AddAction(ActionManager.ActionType.MOVEMENT, false);
        }
    }

    public bool ReachedDestination()
    {
        // If the clone is within stopping distance then they've reached their destination
        if (clone.transform.position.x > clone.destination.x - clone.stoppingDistance &&
            clone.transform.position.x < clone.destination.x + clone.stoppingDistance &&
            clone.transform.position.z > clone.destination.z - clone.stoppingDistance &&
            clone.transform.position.z < clone.destination.z + clone.stoppingDistance)
        {
            return true;
        }
        return false;
    }

    public void GiveObject(GameObject gameObject)
    {
        GameObject instantiated = Instantiate(gameObject, hand);
        TimedObject timedObject = instantiated.GetComponent<TimedObject>();
        ManagerHandler.instance.ActionM.AddAction(timedObject);
    }
}
