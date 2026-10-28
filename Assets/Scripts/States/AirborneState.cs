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

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.blue);
        turnedAround = fighter.facingRight != isFacingRight;
        isFacingRight = fighter.facingRight;

        overrideController = fighter.animator.runtimeAnimatorController as AnimatorOverrideController;
        overrideController["JumpPlaceholder"] = fighter.jumpAnimation;
        fighter.animator.Play("Jump", 0, 0f);
    }

    public void Tick(FighterController fighter)
    {   
        Debug.Log(fighter.velocity);
        fighter.ApplyKnockbackDecay();

        if (turnedAround) fighter.TurnedAround(isFacingRight);

        if (fighter.Input.AttackPressed && fighter.facingRight && fighter.Input.Move.x > 0)
        {
            Debug.Log("forward air");
            overrideController["ForwardAirPlaceholder"] = fighter.forwardAirAnimation;
            fighter.animator.Play("ForwardAir", 0, 0f);
        }
        else if (fighter.Input.AttackPressed && fighter.facingRight && fighter.Input.Move.x < 0)
        {
            Debug.Log("back air");
            overrideController["BackAirPlaceholder"] = fighter.backAirAnimation;
            fighter.animator.Play("BackAir", 0, 0f);
        }
        else if (fighter.Input.AttackPressed && fighter.Input.Move.y < 0)
        {
            Debug.Log("down air");
            overrideController["DownAirPlaceholder"] = fighter.downAirAnimation;
            fighter.animator.Play("DownAir", 0, 0f);
        }
        else if (fighter.Input.AttackPressed && fighter.Input.Move.y > 0)
        {
            Debug.Log("up air");
            overrideController["UpAirPlaceholder"] = fighter.upAirAnimation;
            fighter.animator.Play("UpAir", 0, 0f);
        }
        else if (fighter.Input.SpecialPressed && fighter.Input.Move.y > 0)
        {
            Debug.Log("up B");
            fighter.velocity.y = 15f;
            fighter.Stats.numJumps = 0;
        }

        if (fighter.Input.JumpPressed && fighter.currentJumps > 0)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.currentJumps -= 1;
            fighter.animator.Play("Jump", 0, 0f);
        }
        
        if (fighter.Input.FastFallPressed)
        {  
            float currentGravity = fighter.velocity.y < 0 ? fighter.Stats.gravity * fighter.Stats.fallMultiplier : fighter.Stats.gravity;
            fighter.velocity.y += currentGravity * Time.deltaTime;
        } 
        else
        {
            fighter.velocity.y += fighter.Stats.gravity * Time.deltaTime;
        }

        if (fighter.IsGrounded && fighter.velocity.y <= 0f)
        {
            fighter.ChangeState(new GroundedState());
        }

        float target = fighter.HorizontalInput * fighter.Stats.airSpeed;
        fighter.velocity.x = Mathf.MoveTowards(fighter.velocity.x, target, fighter.Stats.airAccel * Time.deltaTime);
        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + knockback) * Time.deltaTime);
    }

    public void Exit(FighterController fighter)
    {
        fighter.velocity.x = 0f;
        fighter.velocity.y = -5f;
    }
}