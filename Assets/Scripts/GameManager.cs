using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 60f;
    
    private bool isTimerRunning = false;

    private void Start()
    {
        // Starts the timer automatically
        isTimerRunning = true; 
    }

    private void Update()
    {
        if (!isTimerRunning) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            //DisplayTime(timeRemaining);
        }
        else
        {
            Debug.Log("Time has run out!");
            timeRemaining = 0;
            isTimerRunning = false;
            DisplayTime(timeRemaining);
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        // Floors the values to prevent skipping numbers visually
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Formats the text into "00:00"
        //timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

