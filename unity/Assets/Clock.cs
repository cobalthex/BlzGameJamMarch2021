using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public AnalogState Hours;
    public AnalogState Minutes;
    public AnalogState Seconds;

    // clock source?

    void Update()
    {
        var time = DateTime.Now;
        var msec = time.Millisecond / 1000f;
        var sec = (time.Second + msec ) / 60f;
        var min = (time.Minute + sec) / 60f;
        var hour = (time.Hour + min) / 12f % 1;

        if (Hours != null) Hours.Value = Hours.DesiredValue = hour;
        if (Minutes != null) Minutes.Value = Minutes.DesiredValue = min;
        if (Seconds != null) Seconds.Value = Seconds.DesiredValue = sec;
    }
}
