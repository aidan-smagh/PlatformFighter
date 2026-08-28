using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Fighter/Move")]
public class MoveData : ScriptableObject
{
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

    [Header("Hitbox")]
    public Vector3 hitboxSize;
    public Vector3 hitboxOffset;

    [Header("Animation")]
    public AnimationClip animationClip;
}