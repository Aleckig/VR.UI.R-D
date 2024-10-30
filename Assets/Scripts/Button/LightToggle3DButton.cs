using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LightToggle3DButton : MonoBehaviour
{
    [SerializeField] private Light targetLight;         // The light to toggle
    //[SerializeField] private AudioSource audioSource;   // Reference to the AudioSource component
    //[SerializeField] private AudioClip toggleSound;     // The sound clip to play
    [SerializeField] private XRSimpleInteractable buttonInteractable; // Reference to the XR Button Interactable

    private void Start()
    {
        // Set the light to off by default
        targetLight.enabled = false;
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

    private void OnButtonPressed(BaseInteractionEventArgs args)
    {
        // Toggle the light's active state
        targetLight.enabled = !targetLight.enabled;

        // Play the toggle sound
        /*if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }*/
    }
}
