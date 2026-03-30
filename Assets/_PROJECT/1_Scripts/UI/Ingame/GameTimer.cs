using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float maxTime = 60f;
    [SerializeField] private float currentTime = 60f;
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool stopWhenTimeOver = true;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerTextObj;

    [Header("Events")]
    [SerializeField] private UnityEvent onTimeOver;

    private bool isRunning = false;
    private bool isTimeOver = false;

    private void Awake()
    {
        currentTime = maxTime;
        UpdateTimerUI();

        if (startOnAwake == true)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (isRunning == false)
        {
            return;
        }

        if (isTimeOver == true)
        {
            return;
        }

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isTimeOver = true;

            if (stopWhenTimeOver == true)
            {
                isRunning = false;
            }

            UpdateTimerUI();
            onTimeOver.Invoke();
            return;
        }

        UpdateTimerUI();
    }

    public void StartTimer()
    {
        if (isTimeOver == true)
        {
            return;
        }

        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        isTimeOver = false;
        UpdateTimerUI();
    }

    public void SetTime(float newTime)
    {
        currentTime = newTime;

        if (currentTime < 0f)
        {
            currentTime = 0f;
        }

        if (currentTime > maxTime)
        {
            currentTime = maxTime;
        }

        UpdateTimerUI();
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    private void UpdateTimerUI()
    {
        if (timerTextObj == null)
        {
            return;
        }

        int minute = Mathf.FloorToInt(currentTime / 60f);
        int second = Mathf.FloorToInt(currentTime % 60f);

        timerTextObj.text = $"{minute:00}:{second:00}";
    }
}