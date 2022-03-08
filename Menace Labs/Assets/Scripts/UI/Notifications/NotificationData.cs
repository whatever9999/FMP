using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/NotificationData")]
public class NotificationData : ScriptableObject
{
    public NotificationManager.NotificationType type;
    [TextArea]
    public string notificationText;
}
