using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : IFighterState
{

    private bool isFacingRight;
    private bool turnedAround;
    private float horizontalInputDampening = 0.75f;
    private float maxTarget = 0f;

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

        fc.animator.Play("Jump", 0, 0f);

        Debug.Log(fc.currentJumps);
    }

    public void Tick(FighterController fc)
    {   
        fc.ApplyKnockbackDecay();

        if (turnedAround) fc.TurnedAround(isFacingRight);

        if (fc.Input.AttackPressed && fc.facingRight && fc.Input.Move.x > 0)
        {
            MoveData forwardAir = fc.fighter.moveSet.forwardAir;
            fc.animator.Play("ForwardAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.facingRight && fc.Input.Move.x < 0)
        {
            MoveData backAir = fc.fighter.moveSet.backAir;
            fc.animator.Play("BackAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.Input.Move.y < 0)
        {
            MoveData downAir = fc.fighter.moveSet.downAir;
            fc.animator.Play("DownAir", 0, 0f);
        }
        else if (fc.Input.AttackPressed && fc.Input.Move.y > 0)
        {
            MoveData upAir = fc.fighter.moveSet.upAir;
            fc.animator.Play("UpAir", 0, 0f);
        }
        else if (fc.Input.SpecialPressed && fc.Input.Move.y > 0)
        {
            fc.velocity.y = 15f;
            fc.fighter.moveSet.numJumps = 0;
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
        }

        HandleMove(fc);
    }

    public void Exit(FighterController fc)
    {
        fc.velocity.x = 0f;
        fc.velocity.y = -5f;
        fc.currentJumps = fc.fighter.moveSet.numJumps;
    }

    private void HandleMove(FighterController fc)
    {
        float target = fc.HorizontalInput * horizontalInputDampening * fc.fighter.moveSet.airSpeed;
        if (Mathf.Abs(target) > Mathf.Abs(maxTarget)) maxTarget = target;
        maxTarget = Mathf.Abs(maxTarget) * (fc.facingRight ? 1f : -1f);
        fc.velocity.x = Mathf.MoveTowards(fc.velocity.x, maxTarget, fc.fighter.moveSet.airAccel * Time.deltaTime);
        Vector3 knockback = new Vector3(fc.fighter.knockbackVelocity.x, fc.fighter.knockbackVelocity.y, 0f);
        fc.Controller.Move((fc.velocity + knockback) * Time.deltaTime);
    }
}