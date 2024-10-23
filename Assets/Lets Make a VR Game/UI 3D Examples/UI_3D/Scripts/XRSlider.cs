using System;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    public class XRSlider : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
    {
        [Serializable]
        public class ValueChangeEvent : UnityEvent<float> { }

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated")]
        Transform m_Handle = null;

        [SerializeField]
        [Tooltip("The value of the slider")]
        [Range(0.0f, 1.0f)]
        float m_Value = 0.5f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '1'")]
        float m_MaxPosition = 0.5f;

        [SerializeField]
        [Tooltip("The offset of the slider at value '0'")]
        float m_MinPosition = -0.5f;

        [SerializeField]
        [Tooltip("How smooth the slider movement should be (lower = smoother)")]
        float m_Smoothing = 8f;

        [SerializeField]
        [Tooltip("Events to trigger when the slider is moved")]
        ValueChangeEvent m_OnValueChange = new ValueChangeEvent();

        UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor m_Interactor;
        Vector3 m_LastInteractorPosition;

        public float value
        {
            get => m_Value;
            set
            {
                SetValue(value);
                SetSliderPosition(value);
            }
        }

        public ValueChangeEvent onValueChange => m_OnValueChange;

        void Start()
        {
            SetValue(m_Value);
            SetSliderPosition(m_Value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            base.OnDisable();
        }

        void StartGrab(SelectEnterEventArgs args)
        {
            m_Interactor = args.interactorObject;
            m_LastInteractorPosition = m_Interactor.GetAttachTransform(this).position;
            UpdateSliderPosition();
        }

        void EndGrab(SelectExitEventArgs args)
        {
            m_Interactor = null;
            // No additional logic here - slider will stay where it was released
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    UpdateSliderPosition();
                }
            }
        }

        void UpdateSliderPosition()
        {
            if (m_Interactor == null) return;

            var currentInteractorPosition = m_Interactor.GetAttachTransform(this).position;
            var localPosition = transform.InverseTransformPoint(currentInteractorPosition);
            var previousLocalPosition = transform.InverseTransformPoint(m_LastInteractorPosition);

            // Calculate the change in position
            float delta = localPosition.z - previousLocalPosition.z;
            
            // Calculate new value based on current position and movement
            float currentValue = m_Value;
            float range = m_MaxPosition - m_MinPosition;
            float newValue = Mathf.Clamp01(currentValue + (delta / range));

            // Smoothly update to the new value while dragging
            float smoothValue = Mathf.Lerp(currentValue, newValue, Time.deltaTime * m_Smoothing);
            SetValue(smoothValue);
            SetSliderPosition(smoothValue);

            m_LastInteractorPosition = currentInteractorPosition;
        }

        void SetSliderPosition(float value)
        {
            if (m_Handle == null)
                return;

            var handlePos = m_Handle.localPosition;
            handlePos.z = Mathf.Lerp(m_MinPosition, m_MaxPosition, value);
            m_Handle.localPosition = handlePos;
        }

        void SetValue(float value)
        {
            if (Mathf.Abs(m_Value - value) > 0.001f)
            {
                m_Value = value;
                m_OnValueChange.Invoke(m_Value);
            }
        }

        void OnDrawGizmosSelected()
        {
            var sliderMinPoint = transform.TransformPoint(new Vector3(0.0f, 0.0f, m_MinPosition));
            var sliderMaxPoint = transform.TransformPoint(new Vector3(0.0f, 0.0f, m_MaxPosition));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(sliderMinPoint, sliderMaxPoint);
        }

        void OnValidate()
        {
            SetSliderPosition(m_Value);
        }
    }
}