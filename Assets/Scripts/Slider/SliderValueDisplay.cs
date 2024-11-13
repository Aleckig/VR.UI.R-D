using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SliderValueDisplay : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string valueFormat = "Value: {0:F3}"; // F3 for 3 decimal places
    [SerializeField] private string sliderName = "Slider";

    private Coroutine updateDelayCoroutine;
    private float lastLoggedValue = 0f;
    private const float delayTime = 0.5f; // Time in seconds to wait before logging

    private void Start()
    {
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

        slider.onValueChanged.AddListener(OnSliderValueChanged);
        UpdateValueDisplay(slider.value);
    }

    private void OnDestroy()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateValueDisplay(value);

        // Restart the coroutine each time the slider value changes
        if (updateDelayCoroutine != null)
        {
            StopCoroutine(updateDelayCoroutine);
        }
        updateDelayCoroutine = StartCoroutine(LogValueAfterDelay(value));
    }

    private void UpdateValueDisplay(float value)
    {
        valueText.text = string.Format(valueFormat, value);
    }

    private IEnumerator LogValueAfterDelay(float value)
    {
        // Wait for the specified delay time to check if the slider has stopped moving
        yield return new WaitForSeconds(delayTime);

        // Log the value if it has stopped moving
        Debug.Log($"2D Slider value (standstill): {value:F3}");
        lastLoggedValue = value;
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
