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
        fighter.Controller.Move(fighter.velocity * Time.deltaTime);

        if (fighter.IsGrounded())
        {
            fighter.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fighter) { }

    void HandleMove(FighterController fighter)
    {
        float h = 0f;
        if (Keyboard.current.aKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed) h = 1f;
        fighter.Controller.Move(new Vector3(h, 0, 0) * fighter.Stats.runSpeed * Time.deltaTime);
    }
}