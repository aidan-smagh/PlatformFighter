using UnityEngine;
using UnityEngine.InputSystem;

public class OnLedgeState : IFighterState
{
    private readonly Transform ledgePoint;

    private readonly Transform getUpPosition;

    public OnLedgeState(Transform ledgePoint, Transform getUpPosition)
    {
        this.ledgePoint = ledgePoint;
        this.getUpPosition = getUpPosition;
    }
    public void Enter(FighterController fighter)
    {
        fighter.velocity = Vector3.zero;
        fighter.DebugColor(Color.orange);
        Vector3 pos = ledgePoint.position;
        pos.z = 0f;
        fighter.transform.position = pos;
    }

    public void Tick(FighterController fighter)
    {
        bool onRightSide = fighter.transform.position.x > 0;
        //jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.ChangeState(new AirborneState());
            return;
        }
        //drop
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            fighter.ChangeState(new AirborneState());
            return;
        }
        //roll
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            //roll logic here (use lerp)
            fighter.ChangeState(new GroundedState());
            return;
        }
        //get up
        bool getUpPressed = onRightSide
            ? Keyboard.current.aKey.wasPressedThisFrame
            : Keyboard.current.dKey.wasPressedThisFrame;

        if (getUpPressed)
        {
            fighter.velocity = Vector3.zero;
            fighter.Controller.enabled = false;
            Vector3 pos = getUpPosition.position;
            pos.z = 0f;
            fighter.transform.position = pos;
            fighter.Controller.enabled = true;
            fighter.ChangeState(new GroundedState());
            return;
        }
    }

    public void Exit(FighterController fighter) { }

}