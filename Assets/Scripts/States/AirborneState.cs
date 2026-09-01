using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : IFighterState
{
    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.blue);
    }

    public void Tick(FighterController fighter)
    {
        HandleMove(fighter);

        if (Keyboard.current.spaceKey.wasPressedThisFrame && fighter.currentJumps > 0)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.currentJumps -= 1;
        }

        float currentGravity = fighter.velocity.y < 0 ? fighter.Stats.gravity * fighter.Stats.fallMultiplier : fighter.Stats.gravity;
        fighter.velocity.y += currentGravity * Time.deltaTime;
        
        Vector3 horizontal = new Vector3(fighter.HorizontalInput, 0, 0) * fighter.Stats.runSpeed;
        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + horizontal + knockback) * Time.deltaTime);

        if (fighter.IsGrounded())
        {
            fighter.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fighter) { }

    void HandleMove(FighterController fighter)
    {
        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + knockback) * Time.deltaTime);
    }
}