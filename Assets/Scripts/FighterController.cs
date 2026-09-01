using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FighterController : MonoBehaviour
{
    public Vector3 velocity;
    public float currentJumps;
    public float groundCheckDistance = 0.2f;
    public bool facingRight;

    [SerializeField] CharacterController controller;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Fighter fighter;
    [SerializeField] public Fighter grabbedFighter;

    [SerializeField] public GameObject fSmashHitbox;
    [SerializeField] public GameObject dSmashHitbox;
    [SerializeField] public GameObject grabHitbox;
    [SerializeField] public GameObject pummelHitbox;

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

    void Awake()
    {
        for (int i = 0; i < hitboxes.Count; i++)
        {
            var hb = hitboxes[i];
            hb.baseOffset = hb.transform.localPosition;
            hitboxes[i] = hb;
        }

    UpdateHitboxFacing(facingRight);
    }

    void Start()
    {
        currentJumps = fighter.numJumps;
        ChangeState(new GroundedState());
    }

    void Update()
    {
        ReadHorizontalState();
        currentState.Tick(this);
    }

    void ReadHorizontalState()
    {
        float h = 0f;
        if (Keyboard.current.aKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed) h = 1f;

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
}