using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ButtonSequenceTracker : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable buttonA; // Large button
    [SerializeField] private XRSimpleInteractable buttonB; // Medium button
    [SerializeField] private XRSimpleInteractable buttonC; // Small button

    private float lastPressTime = 0f;
    private string lastPressedButton = "";
    private XRSimpleInteractable lastPressedButtonObject = null;

    private void Start()
    {
        if (buttonA == null || buttonB == null || buttonC == null)
        {
            Debug.LogError("One or more buttons are not assigned in the inspector!");
            return;
        }

        buttonA.selectEntered.AddListener((args) => OnButtonPressed(buttonA, "3D Button Large"));
        buttonB.selectEntered.AddListener((args) => OnButtonPressed(buttonB, "3D Button Medium"));
        buttonC.selectEntered.AddListener((args) => OnButtonPressed(buttonC, "3D Button Small"));
    }

    private void OnButtonPressed(XRSimpleInteractable button, string buttonName)
    {
        float currentTime = Time.time;

        // Only log time if it's a different button than the last one pressed
        if (lastPressTime > 0 && !string.IsNullOrEmpty(lastPressedButton) && button != lastPressedButtonObject)
        {
            float timeBetweenPresses = currentTime - lastPressTime;
            Debug.Log($"Time between {lastPressedButton} and {buttonName}: {timeBetweenPresses:F2} seconds");
        }
        else if (lastPressedButton == "")  // Only log first button press once
        {
            //Debug.Log($"First button pressed: {buttonName}");
        }

        // Update the last press info
        lastPressTime = currentTime;
        lastPressedButton = buttonName;
        lastPressedButtonObject = button;
    }

    private void OnEnable()
    {
        if (buttonA != null) buttonA.selectEntered.AddListener((args) => OnButtonPressed(buttonA, "3D Button Large"));
        if (buttonB != null) buttonB.selectEntered.AddListener((args) => OnButtonPressed(buttonB, "3D Button Medium"));
        if (buttonC != null) buttonC.selectEntered.AddListener((args) => OnButtonPressed(buttonC, "3D Button Small"));
    }

    private void OnDisable()
    {
        if (buttonA != null) buttonA.selectEntered.RemoveListener((args) => OnButtonPressed(buttonA, "3D Button Large"));
        if (buttonB != null) buttonB.selectEntered.RemoveListener((args) => OnButtonPressed(buttonB, "3D Button Medium"));
        if (buttonC != null) buttonC.selectEntered.RemoveListener((args) => OnButtonPressed(buttonC, "3D Button Small"));
    }

    public void ResetTimings()
    {
        lastPressTime = 0f;
        lastPressedButton = "";
        lastPressedButtonObject = null;
        Debug.Log("Timing measurements reset");
    }
}