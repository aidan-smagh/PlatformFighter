using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Fighter/Move")]
public class MoveData : ScriptableObject
{
    [System.NonSerialized] public int animationHash;
    public void OnEnable() => animationHash = Animator.StringToHash(moveName);

    public string moveName;

    [Header("Knockback")]
    public float damage;
    public float knockbackScaling;
    public float baseKnockback;
    public float launchAngle;

    [Header("Timing")]
    public float startupTime;
    public float activeTime;
    public float endLag;
    public float hitstunInflicted;

    [Header("Hitbox")]
    public int[] hitboxIndices;

    [Header("Animation")]
    public AnimationClip animationClip;

    [Header("SelfMovement")]
    public float velocityX;
    public float velocityY;
}