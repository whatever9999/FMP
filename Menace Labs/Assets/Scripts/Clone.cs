using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Clone : MonoBehaviour
{
    [SerializeField] private LayerMask clickable;
    
    private NavMeshAgent clone;

    private void Start()
    {
        clone = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, clickable))
            {
                clone.SetDestination(hit.point);
            }
        }
    }
}
