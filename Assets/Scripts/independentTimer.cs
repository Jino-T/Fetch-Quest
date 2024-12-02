using UnityEngine;
using System;

public class IndependentTimer : MonoBehaviour
{
    private DateTime startTime;
    private TimeSpan duration;

    public float timerDuration = 5f; // Duration in seconds
    private bool isTimerRunning;

    public bool hasFinished = false;

    void Start()
    {
        StartTimer(timerDuration);
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if ((DateTime.Now - startTime) >= duration)
            {
                isTimerRunning = false;
                TimerFinished();
            }
        }
    }

    void StartTimer(float seconds)
    {
        startTime = DateTime.Now;
        duration = TimeSpan.FromSeconds(seconds);
        isTimerRunning = true;
    }

    void TimerFinished()
    {
        
    }

    public bool isRunning(){
        return isTimerRunning;
        
    }

    public void StopTimer(){
        isTimerRunning = false;
        TimerFinished();

    }
}
