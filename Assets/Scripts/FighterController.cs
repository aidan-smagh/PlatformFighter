using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FighterController : MonoBehaviour
{
    public Vector3 velocity;
    public float currentJumps;
    public float groundCheckDistance = 0.2f;
    public bool facingRight;
    public bool isShieldActive;
    public bool isPlayerControlled;

    [SerializeField] CharacterController controller;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] Fighter fighter;
    [SerializeField] public Fighter grabbedFighter;
    [SerializeField] public Animator animator;
    

    [SerializeField] public GameObject fSmashHitbox;
    [SerializeField] public GameObject dSmashHitbox;
    [SerializeField] public GameObject grabHitbox;
    [SerializeField] public GameObject pummelHitbox;
    [SerializeField] public GameObject shield;

    [SerializeField] public MovesetData moveSet;

    [System.Serializable]
    public struct FlippableHitbox
    {
        public Transform transform;
        [HideInInspector] public Vector3 baseOffset;
    }

    [SerializeField] private List<FlippableHitbox> hitboxes;

    public CharacterController Controller => controller;
    public Fighter Stats => fighter;
    public float HorizontalInput { get; private set; }

    public IFighterState currentState { get; set; }

    private FighterControls controls;

    public bool IsGrounded()
    {
        float castRadius = controller.radius * 0.9f;
        Vector3 origin = transform.position + controller.center + Vector3.up * 0.1f;
        return Physics.SphereCast(
            origin,
            castRadius,
            Vector3.down,
            out RaycastHit hit,
            controller.height / 2f + groundCheckDistance - 0.1f - castRadius,
            groundMask
        );
    }

    void Awake()
    {
        controls = new FighterControls();
        for (int i = 0; i < hitboxes.Count; i++)
        {
            var hb = hitboxes[i];
            hb.baseOffset = hb.transform.localPosition;
            hitboxes[i] = hb;
        }
        UpdateHitboxFacing(facingRight);

        var baseController = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = new AnimatorOverrideController(baseController);
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    public FighterControls Controls => controls;

    void Start()
    {
        currentJumps = fighter.numJumps;
        ChangeState(new GroundedState());
    }

    void Update()
    {
        if (isPlayerControlled)
        {
            ReadHorizontalState();
        }
        currentState.Tick(this);
    }

    void ReadHorizontalState()
    {
        float h = controls.Player.Move.ReadValue<Vector2>().x;    

        HorizontalInput = h;

        bool previousFacing = facingRight;

        if (h > 0f) facingRight = true;
        else if (h < 0f) facingRight = false;

        if (facingRight != previousFacing)
        {
            UpdateHitboxFacing(facingRight);
        }
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

    void UpdateHitboxFacing(bool facing)
    {
        foreach (var hb in hitboxes)
        {
            Vector3 pos = hb.baseOffset;
            pos.x = facing ? Mathf.Abs(hb.baseOffset.x) : -Mathf.Abs(hb.baseOffset.x);
            hb.transform.localPosition = pos;
        }
    }

    void ApplyKnockbackDecay()
    {
        fighter.knockbackVelocity = Vector2.Lerp(
            fighter.knockbackVelocity, 
            Vector2.zero, 
            fighter.decayRate * Time.deltaTime
        );
    }
}