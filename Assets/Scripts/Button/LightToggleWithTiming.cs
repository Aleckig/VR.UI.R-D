using UnityEngine;
using UnityEngine.UI;

public class LightToggleWithTiming : MonoBehaviour
{
    [SerializeField] private Toggle toggleA; // First toggle button
    [SerializeField] private Toggle toggleB; // Second toggle button
    [SerializeField] private Toggle toggleC; // Third toggle button

    private float lastPressTime = 0f;
    private string lastPressedButton = "";

    void Start()
    {
        // Check if toggles are properly assigned
        if (toggleA == null || toggleB == null || toggleC == null)
        {
            Debug.LogError("One or more toggles are not assigned in the inspector!");
            return;
        }

        // Set up listeners
        toggleA.onValueChanged.AddListener((isOn) => OnTogglePressed(toggleA, "Button Large", isOn));
        toggleB.onValueChanged.AddListener((isOn) => OnTogglePressed(toggleB, "Button Medium", isOn));
        toggleC.onValueChanged.AddListener((isOn) => OnTogglePressed(toggleC, "Button Small", isOn));

        Debug.Log("Toggle listeners setup complete!");
    }

    private void OnTogglePressed(Toggle toggle, string toggleName, bool isOn)
    {
        if (!isOn) return;  // Only proceed if the toggle is being turned on

        float currentTime = Time.time;

        // If this isn't the first button press
        if (lastPressTime > 0 && !string.IsNullOrEmpty(lastPressedButton))
        {
            float timeBetweenPresses = currentTime - lastPressTime;
            Debug.Log($"Time between {lastPressedButton} and {toggleName}: {timeBetweenPresses:F2} seconds");
        }
        else
        {
            Debug.Log($"First button pressed: {toggleName}");
        }

        // Update the last press time and button name
        lastPressTime = currentTime;
        lastPressedButton = toggleName;
    }

    // Optional: Add this method to reset timing if needed
    public void ResetTimings()
    {
        lastPressTime = 0f;
        lastPressedButton = "";
        Debug.Log("Timing measurements reset");
    }
}