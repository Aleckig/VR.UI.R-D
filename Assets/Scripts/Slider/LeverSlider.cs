using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ConfigurableJoint))]
public class SliderLever : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [Header("Slider Settings")]
    [SerializeField] private float minX = -0.5f;
    [SerializeField] private float maxX = 0.5f;
    
    [Header("Physics Settings")]
    [SerializeField] private float jointSpring = 500f;
    [SerializeField] private float jointDamper = 100f;
    
    private ConfigurableJoint joint;
    private Rigidbody rb;
    private Vector3 startPosition;

    protected override void Awake()
    {
        base.Awake();
        
        // Store initial position
        startPosition = transform.localPosition;
        
        // Setup components
        SetupRigidbody();
        SetupJoint();
        
        // Configure the grab interaction
        movementType = MovementType.VelocityTracking;
        trackPosition = true;
        trackRotation = false;
    }

    private void SetupRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false; // Important: Should not be kinematic
        rb.linearDamping = 10f; // Add some drag to prevent unwanted movement
        rb.constraints = RigidbodyConstraints.FreezeRotation | 
                        RigidbodyConstraints.FreezePositionY | 
                        RigidbodyConstraints.FreezePositionZ;
    }

    private void SetupJoint()
    {
        joint = GetComponent<ConfigurableJoint>();
        
        // Configure the joint
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        
        // Lock all rotation
        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        
        // Set limits for X axis movement
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = maxX;
        joint.linearLimit = limit;
        
        // Configure spring and damper
        var drive = new JointDrive
        {
            positionSpring = jointSpring,
            positionDamper = jointDamper,
            maximumForce = float.MaxValue
        };
        
        joint.xDrive = drive;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
        {
            // Clamp position
            Vector3 currentPos = transform.localPosition;
            currentPos.x = Mathf.Clamp(currentPos.x, startPosition.x + minX, startPosition.x + maxX);
            currentPos.y = startPosition.y;
            currentPos.z = startPosition.z;
            transform.localPosition = currentPos;
        }
    }

    // Get normalized value (0-1)
    public float GetNormalizedValue()
    {
        float currentX = transform.localPosition.x - startPosition.x;
        return Mathf.InverseLerp(minX, maxX, currentX);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        rb.linearVelocity = Vector3.zero; // Reset velocity when grabbed
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        if (!rb) rb = GetComponent<Rigidbody>();
        if (!joint) joint = GetComponent<ConfigurableJoint>();
        
        SetupRigidbody();
        if (joint) SetupJoint();
    }
#endif
}