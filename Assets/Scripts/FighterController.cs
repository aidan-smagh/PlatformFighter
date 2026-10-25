using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FighterController : MonoBehaviour
{
    public Vector3 velocity;
    public float currentJumps;
    public float groundCheckDistance = 0.2f;
    public bool facingRight;
    public bool isBlockActive;
    public bool isPlayerControlled;

    [SerializeField] CharacterController controller;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public LayerMask platformMask;
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
    
    [SerializeField] public AnimationClip idleAnimation;
    [SerializeField] public AnimationClip walkAnimation;
    [SerializeField] public AnimationClip runAnimation;
    [SerializeField] public AnimationClip turnAroundAnimation;
    [SerializeField] public AnimationClip blockStartAnimation;
    [SerializeField] public AnimationClip blockHoldAnimation;
    [SerializeField] public AnimationClip blockExitAnimation;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip parrySFX;

    [System.Serializable]
    public struct FlippableHitbox
    {
        public Transform transform;
        [HideInInspector] public Vector3 baseOffset;
    }

    [SerializeField] private List<FlippableHitbox> hitboxes;
    [SerializeField] public List<AttackHitbox> swordHitboxes;

    public struct IgnoredPlatform
    {
        public Collider collider;
        public bool dropping;
    }

    public readonly List<IgnoredPlatform> ignoredPlatforms = new List<IgnoredPlatform>();

    public CharacterController Controller => controller;
    public Fighter Stats => fighter;
    public float HorizontalInput { get; private set; }

    public IFighterState currentState { get; set; }

    public IFighterInput Input { get; set; }

    private FighterControls controls;

    private int combinedMask;

    public Collider ignoredPlatform;

    public bool IsGrounded => CastDown(combinedMask, out _);

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

        var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        overrideController["IdlePlaceholder"] = idleAnimation;
        animator.Play("Idle", 0, 0f);

        if (isPlayerControlled)
            Input = new PlayerFighterInput(Controls);
        else
            Input = new AIFighterInput();
        
        combinedMask = 0;
        combinedMask = groundMask.value | platformMask.value;
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
        ReadHorizontalState();

        UpdatePlatformPassThrough();
        currentState.Tick(this);
    }

    void ReadHorizontalState()
    {
        float h = Input.Move.x; 

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

    public void ApplyKnockbackDecay()
    {
        fighter.knockbackVelocity = Vector2.Lerp(
            fighter.knockbackVelocity, 
            Vector2.zero, 
            fighter.decayRate * Time.deltaTime
        );
    }

    public void turnedAround(bool isFacingRight)
    {
        float yAngle = isFacingRight ? 90f : -90f;
        fighter.transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
    }
}