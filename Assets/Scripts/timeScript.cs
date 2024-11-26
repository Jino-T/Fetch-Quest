using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float initialTime; // Set your desired start time in seconds
    public float timeLeft = 0f;
    public bool isRunning = false;

    public bool debug = false;

    public Text timerText; // Optional: for displaying the time on the UI
    private bool finished;

    public void Initialize(bool start, float setTime, bool beginWithTime)
    {
        isRunning = start;
        initialTime = setTime;
        if (beginWithTime){
            timeLeft = setTime;
            finished = false;
        }else{
            timeLeft = 0f;
            finished = true;

        }
        
    }

    public void Initialize(bool start, float setTime)
    {
        isRunning = start;
        initialTime = setTime;
        timeLeft = setTime;
        finished = false;
    }

    public void Initialize(float setTime)
    {
        isRunning = true;
        initialTime = setTime;
        timeLeft = setTime;
        finished = false;
    }

    void Start()
    {
        //timeLeft = initialTime;
        //finished = false;
    }

    void FixedUpdate()
    {
        if (isRunning)
        {
            timeLeft -= Time.fixedDeltaTime;

            if (timeLeft <= 0)
            {
                timeLeft = 0; // Clamp to 0
                isRunning = false;
                finished = true; // Mark as finished
                TimerFinished();
            }
        }

        if (debug){
            Debug.Log(timeLeft);
        }
        

        //UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.FloorToInt(timeLeft).ToString();
        }
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void StartTimer()
    {
        isRunning = true;
        finished = false; // Reset finished state when starting the timer
    }

    public float GetTimeLeft()
    {
        return timeLeft;
    }

    public bool IsFinished()
    {
        return finished;
    }

    public void ResetTimer()
    {
        timeLeft = initialTime;
        finished = false; // Reset the finished state
    }

    public void EndTimer()
    {
        isRunning = false;
        timeLeft = 0f;
        finished = true; // Mark as finished when ended
    }

    public void AddTime(float addedTime)
    {
        timeLeft += addedTime;
    }

    private void TimerFinished()
    {
        //Debug.Log("Timer finished!");
        // Add additional logic here if needed
    }
}
