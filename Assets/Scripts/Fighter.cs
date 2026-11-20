using UnityEngine;
using System.Collections.Generic;

public class Fighter : MonoBehaviour
{
    public float gravity = -20f;
    public float decayRate = 0.15f;
    public Vector2 knockbackVelocity;
    private float knockbackPowerScaleFactor = 0.15f;
    private float tumbleThreshold = 80f;

    private float damageReduction = 0.1f;

    public float currentPercent = 0f;
    public int stocks = 3;
    [SerializeField] private GameObject spawnPoint;
    public MovesetData moveSet;
    
    public int Stocks => stocks;

    FighterController fc;

    public struct KnockbackData
    {
        public float d;
        public float s;
        public float b;
        public float r;
    }

    HashSet<Fighter> hitTargetsThisSwing = new HashSet<Fighter>();
    
    void Awake()
    {
        fc = GetComponent<FighterController>();
    }

    public void ClearHitTargets()
    {
        hitTargetsThisSwing.Clear();
    }

    public bool HasAlreadyHit(Fighter target)
    {
        return hitTargetsThisSwing.Contains(target);
    }

    public void RegisterHit(Fighter target)
    {
        hitTargetsThisSwing.Add(target);
    }

    public void RemoveStock()
    {
        stocks --;
        currentPercent = 0f;
        knockbackVelocity = Vector2.zero;
        fc.velocity = Vector3.zero;
        CharacterController cc = gameObject.GetComponent<CharacterController>();
        cc.enabled = false;
        transform.position = spawnPoint.transform.position;
        knockbackVelocity = Vector3.zero;
        cc.enabled = true;
        fc.ChangeState(new GroundedState());
    }

    Vector2 AngleToDirection(float angleDegrees, bool facingRight)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        if (!facingRight) x = -x;

        return new Vector2(x, y);
    }

    void ApplyKnockback(Fighter other, Vector2 velocity, bool enterTumble)
    {
        other.knockbackVelocity += velocity;
        //if (enterTumble) other.fighterController.ChangeState(new TumbleState());
    }
    public void ReceiveHit(Fighter other, MoveData move)
    {
        RegisterHit(other);
        bool blockStatus = false;
        FighterController fc = other.GetComponent<FighterController>();
        if (fc.currentState is BlockState block)
        {
            bool inParryWindow = block.currentBlockFrame >= BlockState.ParryStart
                    && block.currentBlockFrame <  BlockState.ParryEnd;

            if (inParryWindow)
            {
                fc.audioSource.PlayOneShot(fc.parrySFX, 0.8f);
                blockStatus = true;
            }
        }
        CalculateMoveHit(other, move, blockStatus);
    }

    public void CalculateMoveHit(Fighter other, MoveData move, bool blockStatus)
    {   
        float moveDamage = move.damage;
        if (blockStatus == true) {
            moveDamage = moveDamage * damageReduction;
        } 

        KnockbackData payload = new KnockbackData 
        { 
            d = moveDamage,
            s = move.knockbackScaling,
            b = move.baseKnockback,
            r = 1f
        };

        double knockbackPower = CalculateKnockback(other, payload, blockStatus);
        
        bool enterTumble = false;
        if (knockbackPower > tumbleThreshold)
        {
            enterTumble = true;
        }

        other.currentPercent += moveDamage;
        Vector2 direction = AngleToDirection(move.launchAngle, fc.facingRight);
        Vector2 knockbackVelocity = direction * (float)knockbackPower * knockbackPowerScaleFactor;
        Debug.Log(knockbackPowerScaleFactor);
        ApplyKnockback(other, knockbackVelocity, enterTumble);
    }

    private double CalculateKnockback(Fighter other, KnockbackData payload, bool blockStatus)
    {
        if (blockStatus == true) return 0;

        double knockbackPower = ((((other.currentPercent / 10 + (other.currentPercent * payload.d) / 20) * (200 / (other.moveSet.weight + 100) * 1.4) + 18) * payload.s) + payload.b) * payload.r;
        return knockbackPower;
    }
}
