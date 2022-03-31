using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] private float visibleDuration = 10.0f;
    private float visibleTimer;

    private void Update()
    {
        visibleTimer += Time.unscaledDeltaTime;

        if (visibleTimer > visibleDuration)
        {
            DeleteNotification();
        }
    }

    public void DeleteNotification()
    {
        ManagerHandler.instance.NotificationM.DeleteNotification(gameObject);
    }
}
