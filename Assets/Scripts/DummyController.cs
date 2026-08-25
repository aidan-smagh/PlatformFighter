using UnityEngine;

public class DummyController : MonoBehaviour
{
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;
    public float fallMultiplier = 2.5f;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private CharacterController controller;

    private Vector3 velocity;

    public bool IsGrounded()
    {
        float castRadius = controller.radius * 0.9f;
        return Physics.SphereCast(
            transform.position + Vector3.up * 0.1f,
            castRadius,
            Vector3.down,
            out RaycastHit hit,
            controller.height / 2f + groundCheckDistance - 0.1f - castRadius,
            groundMask
        );
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (IsGrounded() && velocity.y < 0f)
        {
            velocity.y = -2f; // small downward stick, keeps CharacterController grounded
        }

        float currentGravity = velocity.y < 0f ? gravity * fallMultiplier : gravity;
        velocity.y += currentGravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}