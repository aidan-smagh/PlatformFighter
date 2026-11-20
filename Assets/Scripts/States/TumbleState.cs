using UnityEngine;

public class TumbleState : IFighterState
{

    private Vector3 rotateSpeed = new Vector3(0f, 0f, 60f);
    private bool hasLeftGround = false;

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.black);
    }

    public void Tick(FighterController fc)
    {
        //rotate the character
        fc.transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);

        fc.velocity.y += fc.fighter.gravity * Time.deltaTime;

        Vector3 horizontal = new Vector3(fc.HorizontalInput, 0, 0) * fc.fighter.moveSet.runSpeed;
        Vector3 knockback = new Vector3(fc.fighter.knockbackVelocity.x, fc.fighter.knockbackVelocity.y, 0f);
        fc.Controller.Move((fc.velocity + horizontal + knockback) * Time.deltaTime);

        if (!hasLeftGround)
        {
            if (!fc.IsGrounded) hasLeftGround = true;
        }
        else if (fc.IsGrounded)
        {
            fc.ChangeState(new SupineState());
        }
    }

    public void Exit(FighterController fc)
    {
        
    }
}