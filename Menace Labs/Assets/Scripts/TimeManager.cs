using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum TimeSpeed
    {
        PAUSE,
        PLAY,
        FAST_FORWARD,
    }

    private TimeSpeed previousSpeed;
    private TimeSpeed currentSpeed = TimeSpeed.PLAY;

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
                    Time.timeScale = 2.0f;
                    break;
            }

            currentSpeed = speed;
        }
    }

    // Can be used to resume time to the appropriate speed when unpausing
    public void SetSpeedToPrevious()
    {
        SetTimeSpeed(previousSpeed);
    }
}
