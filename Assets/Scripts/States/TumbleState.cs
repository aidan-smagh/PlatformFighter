using UnityEngine;

public class TumbleState : IFighterState
{

    private Vector3 rotateSpeed = new Vector3(0f, 0f, 60f);
    private bool hasLeftGround = false;

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.black);
    }

    public void Tick(FighterController fighter)
    {
        //rotate the character
        fighter.transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);

        fighter.velocity.y += fighter.Stats.gravity * Time.deltaTime;

        Vector3 horizontal = new Vector3(fighter.HorizontalInput, 0, 0) * fighter.Stats.runSpeed;
        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + horizontal + knockback) * Time.deltaTime);

        if (!hasLeftGround)
        {
            if (!fighter.IsGrounded()) hasLeftGround = true;
        }
        else if (fighter.IsGrounded())
        {
            fighter.ChangeState(new SupineState());
        }
    }

    public void Exit(FighterController fighter)
    {
        
    }
}