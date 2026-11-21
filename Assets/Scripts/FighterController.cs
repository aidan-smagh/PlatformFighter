using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class FighterController : MonoBehaviour
{
    public Vector3 velocity;
    public float currentJumps;
    public float groundCheckDistance = 0.2f;
    public bool facingRight;
    public bool isBlockActive;
    public bool isPlayerControlled;

    [SerializeField] CharacterController controller;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private SkinnedMeshRenderer meshRenderer;
    [SerializeField] public Fighter grabbedFighter;
    [SerializeField] public Animator animator;
    [SerializeField] public RuntimeAnimatorController baseController;
    
    [SerializeField] public GameObject grabHitbox;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip parrySFX;

    [SerializeField] public List<AttackHitbox> swordHitboxes;

    public struct IgnoredPlatform
    {
        public Collider collider;
        public bool dropping;
    }

    public readonly List<IgnoredPlatform> ignoredPlatforms = new List<IgnoredPlatform>();

    public CharacterController Controller => controller;
    public Fighter fighter;
    public float HorizontalInput { get; private set; }

    public IFighterState currentState { get; set; }

    public IFighterInput Input { get; set; }

    private FighterControls controls;

    private int combinedMask;

    public Collider ignoredPlatform;

    public bool IsGrounded => CastDown(combinedMask, out _);

    public bool canTurn;

    void Awake()
    {
        controls = new FighterControls();

        if (isPlayerControlled)
            Input = new PlayerFighterInput(Controls);
        else
            Input = new AIFighterInput();
        
        combinedMask = 0;
        combinedMask = groundMask.value | platformMask.value;
    }

    void Start()
    {
        fighter = GetComponent<Fighter>();
        currentJumps = fighter.moveSet.numJumps;

        var oc = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = oc;
        ApplyAnimationOverrides(oc);

        ChangeState(new GroundedState());
    }

    void Update()
    {
        ReadHorizontalState();

        UpdatePlatformPassThrough();
        currentState.Tick(this);
    }

    public bool CastDown(int mask, out RaycastHit hit)
    {
        float castRadius = controller.radius * 0.9f;
        float startOffset = 0.1f;
        Vector3 origin = transform.position + controller.center + Vector3.up * startOffset;
        float distance = controller.height / 2f - castRadius + startOffset + groundCheckDistance;

        foreach (var h in Physics.SphereCastAll(origin, castRadius, Vector3.down, distance, mask))
        {
            if (IsIgnored(h.collider)) continue;
            hit = h;
            return true;
        }
        hit = default;
        return false;
    }

    public void IgnorePlatform(Collider platform, bool dropping)
    {
        if (IsIgnored(platform)) return;
        Physics.IgnoreCollision(controller, platform, true);
        ignoredPlatforms.Add(new IgnoredPlatform { collider = platform, dropping = dropping });
    }

    public void DropThrough(Collider platform) => IgnorePlatform(platform, true);

    private bool IsIgnored(Collider c)
    {
        foreach (var p in ignoredPlatforms)
            if (p.collider == c) return true;
        return false;
    }

    public void ClearIgnoredPlatforms()
    {
        foreach (var p in ignoredPlatforms)
            if (p.collider != null) Physics.IgnoreCollision(controller, p.collider, false);
        ignoredPlatforms.Clear();
    }

    public bool TryGetPlatform(out Collider platform)
    {
        bool onPlatform = CastDown(platformMask.value, out RaycastHit hit);
        platform = onPlatform ? hit.collider : null;
        return onPlatform;
    }

    void UpdatePlatformPassThrough()
    {
        Bounds b = controller.bounds;

        if (velocity.y > 0f)
        {
            float lookahead = velocity.y * Time.deltaTime + 0.1f;
            float bottom = b.center.y;
            float top = b.max.y + lookahead;
            Vector3 center = new Vector3(b.center.x, (bottom + top) / 2f, b.center.z);
            Vector3 halfExtents = new Vector3(b.extents.x, (top - bottom) / 2f, b.extents.z);

            foreach (var col in Physics.OverlapBox(center, halfExtents, Quaternion.identity, platformMask.value))
                IgnorePlatform(col, false);
        }

        // Restore collision
        for (int i = ignoredPlatforms.Count - 1; i >= 0; i--)
        {
            var p = ignoredPlatforms[i];
            if (p.collider == null) { ignoredPlatforms.RemoveAt(i); continue; }

            bool done = p.dropping
                ? b.max.y < p.collider.bounds.min.y      // head is below the platform's underside
                : b.min.y >= p.collider.bounds.max.y;    // feet are above the platform's top

            if (done)
            {
                Physics.IgnoreCollision(controller, p.collider, false);
                ignoredPlatforms.RemoveAt(i);
            }
        }
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

    private void ReadHorizontalState()
    {
        float h = Input.Move.x; 

        HorizontalInput = h;

        if (!canTurn) return;

        if (h > 0f) facingRight = true;
        else if (h < 0f) facingRight = false;
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

    public void ApplyKnockbackDecay()
    {
        fighter.knockbackVelocity = Vector2.Lerp(
            fighter.knockbackVelocity, 
            Vector2.zero, 
            fighter.decayRate * Time.deltaTime
        );
    }

    public void TurnedAround(bool isFacingRight)
    {
        Debug.Log(facingRight);
        float yAngle = isFacingRight ? 90f : -90f;
        fighter.transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
    }

    public void StartAttack(MoveData move)
    {
        fighter.ClearHitTargets();
        animator.Play(move.moveName, 0, 0f);
    }

    private void ApplyAnimationOverrides(AnimatorOverrideController oc)
    {
        oc["Idle"]  = fighter.moveSet.idleAnimation;
        oc["Walk"]  = fighter.moveSet.walkAnimation;
        oc["Run"]   = fighter.moveSet.runAnimation;
        oc["Jump"] = fighter.moveSet.jumpAnimation;

        oc["ForwardSmash"] = fighter.moveSet.forwardSmash.animationClip;
        oc["DownSmash"] = fighter.moveSet.downSmash.animationClip;
        oc["UpSmash"] = fighter.moveSet.upSmash.animationClip;
        
        /*
        oc["ForwardTilt"] = fighter.moveSet.forwardTilt.animationClip;
        oc["DownTilt"] = fighter.moveSet.downTilt.animationClip;
        oc["UpTilt"] = fighter.moveSet.upTilt.animationClip;

        oc["ForwardAir"] = fighter.moveSet.forwardAir.animationClip;
        oc["BackAir"] = fighter.moveSet.backAir.animationClip;
        oc["DownAir"] = fighter.moveSet.downAir.animationClip;
        oc["UpAir"] = fighter.moveSet.upAir.animationClip;

        oc["UpSpecial"] = fighter.moveSet.upSpecial.animationClip;
        oc["DownSpecial"] = fighter.moveSet.downSpecial.animationClip;
        oc["SideSpecial"] = fighter.moveSet.sideSpecial.animationClip;

        oc["UpThrow"] = fighter.moveSet.upThrow.animationClip;
        oc["DownThrow"] = fighter.moveSet.downThrow.animationClip;
        oc["ForwardThrow"] = fighter.moveSet.forwardThrow.animationClip;
        oc["BackThrow"] = fighter.moveSet.backThrow.animationClip;

        oc["DashAttack"] = fighter.moveSet.dashAttack.animationClip;

        oc["Pummel"] = fighter.moveSet.pummelAnimation;
        oc["Grab"] = fighter.moveSet.grabAnimation;

        oc["BlockStart"] = fighter.moveSet.blockStartAnimation;
        oc["BlockHold"] = fighter.moveSet.blockHold;
        oc["BlockExit"] = fighter.moveSet.blockExit;
        
        oc["TurnAround"] = fighter.moveSet.turnAroundAnimation;
        oc["DoubleJump"] = fighter.moveSet.doubleJumpAnimation;
        oc["GrabRelease"] = fighter.moveSet.releaseAnimation;
        */
    }
}