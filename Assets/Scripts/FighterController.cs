using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    public Vector3 velocity;
    public float currentJumps;
    public float groundCheckDistance = 0.2f;

    [SerializeField] CharacterController controller;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Fighter fighter;
    [SerializeField] public Fighter grabbedFighter;

    [SerializeField] public GameObject testHitbox;
    [SerializeField] public GameObject grabHitbox;
    [SerializeField] public GameObject pummelHitbox;

    public CharacterController Controller => controller;
    public Fighter Stats => fighter;

    IFighterState currentState;

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

    void Start()
    {
        currentJumps = fighter.numJumps;
        ChangeState(new GroundedState());
    }

    void Update()
    {
        currentState.Tick(this);
    }

    public void ChangeState(IFighterState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public void DebugColor(Color color)
    {
        meshRenderer.material.color = color;
    }
}