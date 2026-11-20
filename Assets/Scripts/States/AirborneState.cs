using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : IFighterState
{

    private bool isFacingRight;
    private bool turnedAround;

    AnimatorOverrideController overrideController;

    public AirborneState(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;
    } 

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.blue);
        turnedAround = fc.facingRight != isFacingRight;
        isFacingRight = fc.facingRight;

        overrideController = fc.animator.runtimeAnimatorController as AnimatorOverrideController;
        overrideController["JumpPlaceholder"] = fc.jumpAnimation;
        fc.animator.Play("Jump", 0, 0f);
    }

    public void Tick(FighterController fc)
    {   
        fc.ApplyKnockbackDecay();

        if (turnedAround) fc.TurnedAround(isFacingRight);

        if (fc.Input.AttackPressed && fc.facingRight && fc.Input.Move.x > 0)
        {
            overrideController["ForwardAirPlaceholder"] = fc.forwardAirAnimation;
            fc.animator.Play("ForwardAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.facingRight && fc.Input.Move.x < 0)
        {
            overrideController["BackAirPlaceholder"] = fc.backAirAnimation;
            fc.animator.Play("BackAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.Input.Move.y < 0)
        {
            overrideController["DownAirPlaceholder"] = fc.downAirAnimation;
            fc.animator.Play("DownAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.Input.Move.y > 0)
        {
            overrideController["UpAirPlaceholder"] = fc.upAirAnimation;
            fc.animator.Play("UpAir", 0, 0f);
        }
        else if (fc.Input.SpecialPressed && fc.Input.Move.y > 0)
        {
            fc.velocity.y = 15f;
            fc.fighter.moveSet.numJumps = 0;
        }

        if (fc.Input.JumpPressed && fc.currentJumps > 0)
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
            fc.velocity.y += fc.fighter.gravity * Time.deltaTime;
        }

        if (fc.IsGrounded && fc.velocity.y <= 0f)
        {
            fc.ChangeState(new GroundedState());
        }

        float target = fc.HorizontalInput * fc.fighter.moveSet.airSpeed;
        fc.velocity.x = Mathf.MoveTowards(fc.velocity.x, target, fc.fighter.moveSet.airAccel * Time.deltaTime);
        Vector3 knockback = new Vector3(fc.fighter.knockbackVelocity.x, fc.fighter.knockbackVelocity.y, 0f);
        fc.Controller.Move((fc.velocity + knockback) * Time.deltaTime);
    }

    public void Exit(FighterController fc)
    {
        fc.velocity.x = 0f;
        fc.velocity.y = -5f;
    }
}