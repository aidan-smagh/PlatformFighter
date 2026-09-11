using UnityEngine;

/// <summary>
/// Attach this to each bone in the cape's bone chain (except the root/anchor bone,
/// which should stay a normal child of the shoulder/spine bone with no script on it).
/// Each SpringBone lags behind its parent's rotation, then eases back toward
/// its "rest" pose using a spring-damper formula.
/// </summary>
public class SpringBone : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The bone this one should hang from / chase. Usually the parent transform.")]
    public Transform parentBone;

    [Tooltip("Child bone this one points toward, used to preserve bone length. Optional.")]
    public Transform childBone;

    [Header("Spring Settings")]
    [Tooltip("How quickly the bone snaps back toward its rest pose. Higher = stiffer.")]
    [Range(0f, 1f)] public float stiffness = 0.1f;

    [Tooltip("How quickly motion settles. Higher = less oscillation/wobble.")]
    [Range(0f, 1f)] public float damping = 0.3f;

    [Tooltip("Extra downward pull, simulating gravity on the cape.")]
    public float gravity = 0.5f;

    [Tooltip("Max distance this bone's tip can drift from its rest position, to avoid extreme stretching during fast motion.")]
    public float maxStretch = 0.3f;

    // Internal state
    private Vector3 currentTipPos;
    private Vector3 previousTipPos;
    private Vector3 boneAxis; // local direction from this bone to its child, cached at start
    private float boneLength;

    void Start()
    {
        if (childBone != null)
        {
            boneLength = Vector3.Distance(transform.position, childBone.position);
            boneAxis = transform.InverseTransformDirection(childBone.position - transform.position).normalized;
        }
        else
        {
            // Fallback: assume bone points down its local Y axis with a default length
            boneLength = 0.2f;
            boneAxis = Vector3.down;
        }

        currentTipPos = transform.position + transform.TransformDirection(boneAxis) * boneLength;
        previousTipPos = currentTipPos;
    }

    void LateUpdate()
    {
        if (parentBone == null) return;

        // Where the tip WOULD be if this bone just rigidly followed its parent's current rotation
        Vector3 restTipPos = transform.position + transform.TransformDirection(boneAxis) * boneLength;

        // Verlet-style integration: velocity is implied by (current - previous)
        Vector3 velocity = (currentTipPos - previousTipPos) * (1f - damping);

        // Pull toward rest position based on stiffness, add velocity carry-over, add gravity
        Vector3 targetTipPos = currentTipPos + velocity;
        targetTipPos = Vector3.Lerp(targetTipPos, restTipPos, stiffness);
        targetTipPos += Vector3.down * gravity * Time.deltaTime;

        // Clamp so the tip doesn't stretch too far from the bone's actual root position
        Vector3 fromRoot = targetTipPos - transform.position;
        if (fromRoot.magnitude > boneLength + maxStretch)
        {
            fromRoot = fromRoot.normalized * (boneLength + maxStretch);
            targetTipPos = transform.position + fromRoot;
        }

        previousTipPos = currentTipPos;
        currentTipPos = targetTipPos;

        // Rotate this bone so its local axis points at the simulated tip position
        Vector3 currentAxisWorld = transform.TransformDirection(boneAxis);
        Vector3 desiredAxisWorld = (currentTipPos - transform.position).normalized;

        Quaternion rotationFix = Quaternion.FromToRotation(currentAxisWorld, desiredAxisWorld);
        transform.rotation = rotationFix * transform.rotation;
    }
}