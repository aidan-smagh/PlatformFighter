using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveset", menuName = "Fighter/Moveset")]
public class MovesetData : ScriptableObject
{
    [Header("Moves")]
    public MoveData jab;

    public MoveData forwardSmash;
    public MoveData downSmash;
    public MoveData upSmash;

    public MoveData forwardTilt;
    public MoveData downTilt;
    public MoveData upTilt;

    public MoveData forwardAir;
    public MoveData backAir;
    public MoveData upAir;
    public MoveData downAir;

    public MoveData upSpecial;
    public MoveData sideSpecial;
    public MoveData downSpecial;

    public MoveData upThrow;
    public MoveData downThrow;
    public MoveData forwardThrow;
    public MoveData backThrow;

    public MoveData pummel;

    public MoveData dashAttack;

    [Header("Stats")]
    public float weight;
    public float runSpeed;
    public float fallMultiplier;
    public float numJumps;
    public float airAccel;
    public float airSpeed;
    public float jumpHeight;

    [Header("Inherent Animations")]
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip runAnimation;
    public AnimationClip jumpAnimation;
    public AnimationClip turnAroundAnimation;

    [Header("Block Animations")]
    public AnimationClip blockStartAnimation;
    public AnimationClip blockHoldAnimation;
    public AnimationClip blockExitAnimation;
}