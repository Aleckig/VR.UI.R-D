using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string valueFormat = "Value: {0:F2}"; // F2 means 2 decimal places

    private void Start()
    {
        // Check if components are assigned
        if (slider == null)
        {
            Debug.LogError("Slider not assigned to SliderValueDisplay!");
            return;
        }

        if (valueText == null)
        {
            Debug.LogError("TextMeshProUGUI component not assigned to SliderValueDisplay!");
            return;
        }

        // Add listener for slider value changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Initialize text with starting value
        UpdateValueDisplay(slider.value);
    }

    private void OnDestroy()
    {
        // Clean up listener when object is destroyed
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateValueDisplay(value);
    }

    private void UpdateValueDisplay(float value)
    {
        // Update UI Text
        valueText.text = string.Format(valueFormat, value);
        
        // Log to console
        Debug.Log($"Slider value: {value:F2}");
    }

    // Public method to get current slider value
    public float GetSliderValue()
    {
        return slider.value;
    }

    // Public method to set slider value
    public void SetSliderValue(float newValue)
    {
        slider.value = newValue;
        UpdateValueDisplay(newValue);
    }
}