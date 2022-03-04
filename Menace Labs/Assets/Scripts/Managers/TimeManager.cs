using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    private const float timerCheck = 1.0f;
    private float timerTimer;
    private int currentTime = (int)Times.DAY_START;
    public int GetCurrentTime() { return currentTime; }
    // Return the time in seconds between the current time and the time passed in
    // If time has lapsed we need to return time to mightnight plus the current time (will only account for a single lapse)
    public int TimeSince(int time) { return currentTime >= time ? currentTime - time : (currentTime + (int)Times.MIDNIGHT - time); }

    private TextMeshProUGUI timeText;

    [SerializeField] TimeTrigger[] timeTriggers;

    public enum TimeSpeed
    {
        PAUSE,
        PLAY,
        FAST_FORWARD,
        NUM_TIME_SPEEDS,
    }
    public enum Times
    {
        DAY_START = 480, // 8am
        MID_AFTERNOON = 840, // 2pm
        NIGHT_START = 1080, // 6pm
        MIDNIGHT = 1640,
    }

    private TimeSpeed previousSpeed;
    private TimeSpeed currentSpeed = TimeSpeed.PLAY;
    public TimeSpeed GetSpeed() { return currentSpeed; }

    private List<EventManager.EventType> morningStartTriggers = new List<EventManager.EventType>();
    private List<EventManager.EventType> midAfternoonTriggers = new List<EventManager.EventType>();
    private List<EventManager.EventType> eveningStartTriggers = new List<EventManager.EventType>();

    private void Start()
    {
        timeText = GetComponent<TextMeshProUGUI>();

        // Create lists of triggers to check at event times
        for (int i = 0; i < timeTriggers.Length; i++)
        {
            switch (timeTriggers[i].time)
            {
                case Times.DAY_START:
                    morningStartTriggers.Add(timeTriggers[i].eventType);
                    break;
                case Times.MID_AFTERNOON:
                    midAfternoonTriggers.Add(timeTriggers[i].eventType);
                    break;
                case Times.NIGHT_START:
                    eveningStartTriggers.Add(timeTriggers[i].eventType);
                    break;
            }
        }

        // Ensure the clock is correct on start
        UpdateUI();
    }

    private void Update()
    {
        CheckHotkeys();

        timerTimer += Time.deltaTime;

        if (timerTimer > timerCheck)
        {
            currentTime += (int)Time.timeScale;

            UpdateUI();
            CheckTriggers();

            timerTimer = 0.0f;
        }

        // Reset current time once 24h is reached
        if (currentTime > (int)Times.MIDNIGHT)
        {
            timerTimer = 0;
        }
    }

    // Format time
    private void UpdateUI()
    {
        string timeString = "";
        if ((currentTime / 60) < 10)
        {
            timeString += "0";
        }
        timeString += (currentTime / 60) + ":";
        if ((currentTime % 60) < 10)
        {
            timeString += "0";
        }
        timeString += (currentTime % 60);
        timeText.text = timeString;
    }

    // p = pause, 1 = play, 2 = FF
    private void CheckHotkeys()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetTimeSpeed(TimeSpeed.PAUSE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTimeSpeed(TimeSpeed.PLAY);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTimeSpeed(TimeSpeed.FAST_FORWARD);
        }
    }

    private void CheckTriggers()
    {
        List<EventManager.EventType> eventsToCheck = new List<EventManager.EventType>();

        // If the current time aligns with any significant times then we'll be checking those events
        switch(currentTime)
        {
            case (int)Times.DAY_START:
                eventsToCheck = morningStartTriggers;
                break;
            case (int)Times.MID_AFTERNOON:
                eventsToCheck = midAfternoonTriggers;
                break;
            case (int)Times.NIGHT_START:
                eventsToCheck = eveningStartTriggers;
                break;
        }

        // Check the events we have a list of
        for (int i = 0; i < eventsToCheck.Count; i++)
        {
            ManagerHandler.instance.EventM.CheckEventTrigger(eventsToCheck[i]);
        }
    }

    // Provide function using integers for buttons to use
    public void SetTimeSpeed(int speed)
    {
        TimeSpeed newSpeed = TimeSpeed.FAST_FORWARD;
        if (speed == 0) newSpeed = TimeSpeed.PAUSE;
        else if (speed == 1) newSpeed = TimeSpeed.PLAY;

        SetTimeSpeed(newSpeed);
    }

    public void SetTimeSpeed(TimeSpeed speed)
    {
        if (currentSpeed != speed)
        {
            // Track the previous time for when we unpause
            previousSpeed = currentSpeed;

            switch (speed)
            {
                case TimeSpeed.PAUSE:
                    Time.timeScale = 0.0f;
                    break;
                case TimeSpeed.PLAY:
                    Time.timeScale = 1.0f;
                    break;
                case TimeSpeed.FAST_FORWARD:
                    Time.timeScale = 3.0f;
                    break;
            }

            currentSpeed = speed;

            ManagerHandler.instance.PerformanceMetric.IncrementPerformanceAttribute(PerformanceMetric.PerformanceData.SPEED_CHANGES);
        }
    }

    // Can be used to resume time to the appropriate speed when unpausing
    public void SetSpeedToPrevious()
    {
        SetTimeSpeed(previousSpeed);
    }
}

[System.Serializable]
public struct TimeTrigger
{
    public TimeManager.Times time;
    public EventManager.EventType eventType;
}