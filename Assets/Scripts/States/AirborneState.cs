using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : IFighterState
{

    private bool isFacingRight;
    private bool turnedAround;

    public AirborneState(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;
    } 

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.blue);
        turnedAround = fighter.facingRight != isFacingRight;
        isFacingRight = fighter.facingRight;
    }

    public void Tick(FighterController fighter)
    {   
        fighter.ApplyKnockbackDecay();

        if (turnedAround) fighter.turnedAround(isFacingRight);

        if (fighter.Input.JumpPressed && fighter.currentJumps > 0)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.currentJumps -= 1;
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

        Vector3 horizontal = new Vector3(fighter.HorizontalInput, 0, 0) * fighter.Stats.runSpeed;
        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + horizontal + knockback) * Time.deltaTime);

        if (fighter.IsGrounded)
        {
            fighter.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fighter) { }
}