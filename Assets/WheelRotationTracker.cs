using UnityEngine;
using UnityEngine.UI;

public class WheelRotationTracker : MonoBehaviour
{

    [SerializeField] private float wheelValue;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform wheel;

    void Start()
    {

    }

    void Update()
    {
        wheelValue = wheel.localEulerAngles.y;
        if (wheelValue <= 180 && wheelValue >= 0)
        {
            slider.value = Mathf.Abs(wheelValue / 180f);
        }
    }
}
