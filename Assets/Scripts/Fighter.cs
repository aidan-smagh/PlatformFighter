using UnityEngine;
using System.Collections.Generic;

public class Fighter : MonoBehaviour
{
    public float runSpeed = 8f;
    public float jumpHeight = 2.5f;
    public float fallMultiplier = 2.5f;
    public float gravity = -20f;
    public float numJumps = 1;
    public float weight = 100;
    public float decayRate = 0.15f;
    public Vector2 knockbackVelocity;
    public float knockbackPowerScaleFactor = 0f;
    public float tumbleThreshold = 80f;
    public float airAccel = 800f;
    public float airSpeed = 100f;

    private float damageReduction = 0.1f;

    [SerializeField] public float currentPercent = 0f;
    [SerializeField] public int stocks = 3;
    [SerializeField] GameObject spawnPoint;
    
    public int Stocks => stocks;
    GameObject fighter;

    Fighter dummy;
    FighterController fighterController;

    public struct KnockbackData
    {
        public float d;
        public float s;
        public float b;
        public float r;
    }

    HashSet<Fighter> hitTargetsThisSwing = new HashSet<Fighter>();

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

    void Awake()
    {
        fighter = gameObject;
        fighterController = GetComponent<FighterController>();
    }

    public void RemoveStock()
    {
        stocks --;
        currentPercent = 0f;
        knockbackVelocity = Vector2.zero;
        fighterController.velocity = Vector3.zero;
        CharacterController cc = gameObject.GetComponent<CharacterController>();
        cc.enabled = false;
        fighter.transform.position = spawnPoint.transform.position;
        knockbackVelocity = Vector3.zero;
        cc.enabled = true;
        fighterController.ChangeState(new GroundedState());
    }

    double CalculateKnockback(Fighter other, KnockbackData payload, bool blockStatus)
    {
        if (blockStatus == true) return 0;

        double knockbackPower = ((((other.currentPercent / 10 + (other.currentPercent * payload.d) / 20) * (200 / (other.weight + 100) * 1.4) + 18) * payload.s) + payload.b) * payload.r;
        Debug.Log("knockbackPower" + knockbackPower);
        return knockbackPower;
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
        Vector2 direction = AngleToDirection(move.launchAngle, fighterController.facingRight);
        Vector2 knockbackVelocity = direction * (float)knockbackPower * knockbackPowerScaleFactor;
        Debug.Log(knockbackPowerScaleFactor);
        ApplyKnockback(other, knockbackVelocity, enterTumble);
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
}
