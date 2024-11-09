using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SceneResetButton : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable resetButton;
    
    private void Start()
    {
        if (resetButton == null)
        {
            Debug.LogError("Reset button not assigned in the inspector!");
            return;
        }

        // Set up the button press listener
        resetButton.selectEntered.AddListener(OnResetButtonPressed);
    }

    private void OnResetButtonPressed(BaseInteractionEventArgs args)
    {
        Debug.Log("Reset button pressed - Reloading scene...");
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnEnable()
    {
        if (resetButton != null)
        {
            resetButton.selectEntered.AddListener(OnResetButtonPressed);
        }
    }

    private void OnDisable()
    {
        if (resetButton != null)
        {
            resetButton.selectEntered.RemoveListener(OnResetButtonPressed);
        }
    }
}