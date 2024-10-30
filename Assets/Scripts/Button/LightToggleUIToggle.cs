using UnityEngine;
using UnityEngine.UI;

public class LightToggleUIToggle : MonoBehaviour
{
    [SerializeField] private Light targetLight; // Light to toggle
    [SerializeField] private Toggle lightToggle; // UI Toggle to control light
    [SerializeField] private AudioSource audioSource; // Audio source for sound effect
    [SerializeField] private AudioClip toggleSound; // Sound clip to play

    void Start()
    {
        // Set light state to match Toggle's initial value
        targetLight.enabled = lightToggle.isOn;
        
        // Add listener to update light and play sound when toggled
        lightToggle.onValueChanged.AddListener(ToggleLight);
    }

    private void ToggleLight(bool isOn)
    {
        targetLight.enabled = isOn;

        // Play the toggle sound effect
        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }
}

