using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeRemaining = 60f;
    [SerializeField] private GameObject fighterUIContainer;
    
    private bool isTimerRunning = false;
    private FighterControls controls;
    private bool isPaused = false;

    Dictionary<Fighter, TextMeshProUGUI> fighterPercents = new Dictionary<Fighter, TextMeshProUGUI>();

    private void Awake()
    {
        controls = new FighterControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }
    private void Start()
    {
        // Starts the timer automatically
        isTimerRunning = true; 
        GameObject[] fighters = GameObject.FindGameObjectsWithTag("Fighter");
        foreach (var fighter in fighters)
        {
            Fighter entry = fighter.GetComponent<Fighter>();
            GameObject textObj = new GameObject("FighterPercent");
            textObj.transform.SetParent(GameObject.Find(fighterUIContainer.name).transform, false);

            textObj.AddComponent<RectTransform>();
            textObj.AddComponent<CanvasRenderer>();
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            //tmpText.text = "number here";
            tmpText.fontSize = 32;
            fighterPercents.Add(entry, tmpText);
        }
    }

    private void Update()
    {
        foreach (var entry in fighterPercents)
        {
            if (entry.Key == null) continue;
            entry.Value.text = entry.Key.currentPercent + "%";
        }
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

        if (!isPaused)
        {
            if (controls.Player.Pause.WasPressedThisFrame())
            {
                Time.timeScale = 0;
                isPaused = true;
            }
        } 
        else
        {
            if (controls.Player.Pause.WasPressedThisFrame()) {
                Time.timeScale = 1;
                isPaused = false;
            }
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

