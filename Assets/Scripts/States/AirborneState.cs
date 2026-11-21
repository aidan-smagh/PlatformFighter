using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : IFighterState
{

    private bool isFacingRight;
    private bool turnedAround;
    private float horizontalInputDampening = 0.75f;

    private enum Aerial { ForwardAir, BackAir, UpAir, DownAir, NeutralAir }

    public AirborneState(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;
    } 

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.blue);
        turnedAround = fc.facingRight != isFacingRight;
        isFacingRight = fc.facingRight;

        fc.animator.Play("Jump", 0, 0f);
        fc.canTurn = false;
    }

    public void Tick(FighterController fc)
    {   
        fc.ApplyKnockbackDecay();

        if (turnedAround) fc.TurnedAround(isFacingRight);
        turnedAround = false;

        if (fc.Input.AttackPressed)
        {
            MoveData move = fc.fighter.moveSet.aerials[(int)AttackLookup(fc.Input.Move, fc)];
            fc.StartAttack(move);
        }

        if (fc.Input.JumpDown && fc.currentJumps > 0)
        {
            fc.velocity.y = Mathf.Sqrt(fc.fighter.moveSet.jumpHeight * -2f * fc.fighter.gravity);
            fc.currentJumps -= 1;
            fc.animator.Play("Jump", 0, 0f);
        }
        
        if (fc.Input.FastFallPressed)
        {  
            float currentGravity = fc.velocity.y < 0 ? fc.fighter.gravity * fc.fighter.moveSet.fallMultiplier : fc.fighter.gravity;
            fc.velocity.y += currentGravity * Time.deltaTime;
        } 
        else
        {
            fc.velocity.y += fc.fighter.gravity * fc.fighter.moveSet.fallSpeed * Time.deltaTime;
        }

        if (fc.IsGrounded && fc.velocity.y <= 0f)
        {
            fc.ChangeState(new GroundedState());
            return;
        }

        HandleMove(fc);
    }

    public void Exit(FighterController fc)
    {
        fc.velocity.x = 0f;
        fc.velocity.y = -5f;
        fc.currentJumps = fc.fighter.moveSet.numJumps;
        fc.canTurn = true;
    }

    private void HandleMove(FighterController fc)
    {
        float target = fc.HorizontalInput * horizontalInputDampening * fc.fighter.moveSet.airSpeed;
        if (Mathf.Abs(fc.HorizontalInput) > 0.2f)
            fc.velocity.x = Mathf.MoveTowards(fc.velocity.x, target, fc.fighter.moveSet.airAccel * Time.deltaTime);
        Vector3 knockback = new Vector3(fc.fighter.knockbackVelocity.x, fc.fighter.knockbackVelocity.y, 0f);
        fc.Controller.Move((fc.velocity + knockback) * Time.deltaTime);
    }

    private Aerial AttackLookup(Vector2 attackInput, FighterController fc)
    {
        float relX = attackInput.x * (fc.facingRight ? 1f : -1f);
        const float threshold = 0.4f;

        if (Mathf.Abs(attackInput.y) < threshold && Mathf.Abs(attackInput.x) < threshold) return Aerial.NeutralAir;

        if (Mathf.Abs(attackInput.y) > Mathf.Abs(relX))
            return attackInput.y > 0 ? Aerial.UpAir : Aerial.DownAir;       
        return relX > 0 ? Aerial.ForwardAir : Aerial.BackAir;
    }
}