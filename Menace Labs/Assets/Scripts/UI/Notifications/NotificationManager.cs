using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public enum NotificationType
    {
        TEST,
        ILLNESS,
        AGE_UP,
        FOOD_DELIVERY,
        FIRE,
        TEST_FINISHED,
        FOOD_SUPPLY_SORTED,
        ELECTROCUTION,
        CLONE_ON_FIRE,
        THERE_IS_FIRE,
        NUM_NOTIFICATION_TYPES,
    }

    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private NotificationData[] notificationDatas;

    [SerializeField] private int maxNotifications = 3;

    private Dictionary<NotificationType, NotificationData> notifications = new Dictionary<NotificationType, NotificationData>();

    private List<GameObject> currentNotifications = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < notificationDatas.Length; i++)
        {
            notifications.Add(notificationDatas[i].type, notificationDatas[i]);
        }
    }

    public void AddNotification(NotificationType notificationType)
    {
        // If were at the notification limit, remove the oldest one
        if (currentNotifications.Count >= maxNotifications)
        {
            GameObject removeNotification = currentNotifications[0];
            currentNotifications.Remove(removeNotification);
            Destroy(removeNotification);
        }

        // Add the new notification
        GameObject notification = Instantiate(notificationPrefab, transform);
        currentNotifications.Add(notification);

        // Set the text in the notification for the type's data
        TextMeshProUGUI notificationText = notification.GetComponentInChildren<TextMeshProUGUI>();
        NotificationData data;
        bool gotData = notifications.TryGetValue(notificationType, out data);

        if (gotData)
        {
            notificationText.text = data.notificationText;
        }
        else
        {
            Debug.LogError("Failed to get notification of type: " + notificationType);
        }
    }

    public void DeleteNotification(GameObject notificationObject)
    {
        currentNotifications.Remove(notificationObject);
        Destroy(notificationObject);
    }
}
