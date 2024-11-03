using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class TimerController3DButton : MonoBehaviour
{
    [Header("XR Interaction")]
    [SerializeField] private XRSimpleInteractable buttonInteractable;

    [Header("Timer Settings")]
    [SerializeField] private string timerName = "Timer";
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonSound;

    private float currentTime = 0f;
    private bool isRunning = false;
    private TimerState currentState = TimerState.Reset;

    private enum TimerState
    {
        Reset,
        Running,
        Stopped
    }

    private void OnEnable()
    {
        // Register the button press event
        buttonInteractable.selectEntered.AddListener(OnButtonPressed);
    }

    private void OnDisable()
    {
        // Unregister the button press event
        buttonInteractable.selectEntered.RemoveListener(OnButtonPressed);
    }

    private void Start()
    {
        UpdateTimerDisplay();
        Debug.Log($"[{timerName}] Timer initialized");
    }

    private void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void OnButtonPressed(BaseInteractionEventArgs args)
    {
        // Play button sound
        if (audioSource != null && buttonSound != null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        switch (currentState)
        {
            case TimerState.Reset:
                // Start timer
                isRunning = true;
                currentState = TimerState.Running;
                Debug.Log($"[{timerName}] Timer Started");
                break;

            case TimerState.Running:
                // Stop timer
                isRunning = false;
                currentState = TimerState.Stopped;
                Debug.Log($"[{timerName}] Timer Stopped at: {FormatTime(currentTime)}");
                break;

            case TimerState.Stopped:
                // Reset timer
                isRunning = false;
                currentTime = 0f;
                currentState = TimerState.Reset;
                UpdateTimerDisplay();
                Debug.Log($"[{timerName}] Timer Reset");
                break;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            string displayText = $"{timerName}: {FormatTime(currentTime)}";
            timerText.text = displayText;
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        int milliseconds = (int)((timeInSeconds * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}