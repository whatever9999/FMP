using UnityEngine;
using UnityEngine.AI;

public class Clone : MonoBehaviour
{
    private NavMeshAgent clone;

    private void Start()
    {
        clone = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ActionManager.instance.AddAction(ActionManager.ActionType.MOVEMENT);
        }
    }
}
