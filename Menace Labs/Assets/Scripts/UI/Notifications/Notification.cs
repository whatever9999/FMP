using UnityEngine;

public class Notification : MonoBehaviour
{
    public void DeleteNotification()
    {
        ManagerHandler.instance.NotificationM.DeleteNotification(gameObject);
    }
}
