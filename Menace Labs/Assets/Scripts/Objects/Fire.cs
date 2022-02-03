using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField] float timeToCatchFire = 2.0f;

    private float timeInFire;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Clone"))
        {
            timeInFire += Time.deltaTime;

            if (timeInFire >= timeToCatchFire) ManagerHandler.instance.clone.SetOnFire(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        timeInFire = 0.0f;
    }
}
