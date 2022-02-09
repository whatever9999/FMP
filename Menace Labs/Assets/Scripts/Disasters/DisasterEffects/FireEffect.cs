using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Scriptable Objects/DisasterEffects/Fire")]
public class FireEffect : DisasterEffect
{
    public int number;
    public GameObject firePrefab;

    const int RANGE = 10;

    public override void TriggerEffect()
    {
        for (int i = 0; i < number; i++)
        {
            // Get a random position on the navmesh to spawn a puddle
            NavMeshHit hit;
            NavMesh.SamplePosition(Vector3.zero + (Random.insideUnitSphere * RANGE), out hit, RANGE, NavMesh.AllAreas);

            Instantiate(firePrefab, hit.position, Quaternion.identity);
        }
    }
}