using UnityEngine;
using UnityEngine.UI;

public class WheelRotationTracker : MonoBehaviour
{
    [SerializeField] private float wheelValue;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform wheel;
    [SerializeField] private AudioSource rotationSound;

    // This controls how many full wheel rotations it takes to fill the slider from 0 to 1
    [SerializeField] private float rotationsPerSlider = 5f;

    private float accumulatedRotation;
    private bool isRotating;

    void Update()
    {
        // Get the current Y rotation in degrees
        float currentWheelAngle = wheel.localEulerAngles.y;

        // Calculate the change in rotation since the last frame
        float rotationChange = Mathf.DeltaAngle(wheelValue, currentWheelAngle);
        wheelValue = currentWheelAngle;

        // Check if there was rotation (wheel is being turned)
        isRotating = Mathf.Abs(rotationChange) > Mathf.Epsilon;

        // If rotating, start the sound if it's not already playing
        if (isRotating && !rotationSound.isPlaying)
        {
            rotationSound.Play();
        }
        // If not rotating, stop the sound
        else if (!isRotating && rotationSound.isPlaying)
        {
            rotationSound.Stop();
        }

        // Accumulate the rotation change, positive for clockwise and negative for counterclockwise
        accumulatedRotation += rotationChange;

        // Clamp accumulatedRotation to keep the slider value within the 0 to 1 range
        accumulatedRotation = Mathf.Clamp(accumulatedRotation, 0, 360f * rotationsPerSlider);

        // Update the slider value based on the accumulated rotation and target rotations per slider
        slider.value = accumulatedRotation / (360f * rotationsPerSlider);
    }
}
