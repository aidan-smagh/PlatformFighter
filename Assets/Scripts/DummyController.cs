using UnityEngine;

public class DummyController : MonoBehaviour
{
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;
    public float fallMultiplier = 2.5f;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Fighter fighter;

    public Vector3 velocity;
    public Vector2 knockbackVelocity;

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

    public void Update()
    {
        if (IsGrounded() && velocity.y < 0f)
        {
            velocity.y = -2f; // small downward stick, keeps CharacterController grounded
        }

        float currentGravity = velocity.y < 0f ? gravity * fallMultiplier : gravity;
        velocity.y += currentGravity * Time.deltaTime;

        Vector3 totalVelocity = velocity + new Vector3(fighter.knockbackVelocity.x, fighter.knockbackVelocity.y, 0f);
        controller.Move(totalVelocity * Time.deltaTime);

        knockbackVelocity = Vector2.Lerp(fighter.knockbackVelocity, Vector2.zero, fighter.decayRate * Time.deltaTime);
    }
}