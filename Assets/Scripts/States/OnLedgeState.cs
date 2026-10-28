using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class OnLedgeState : IFighterState
{
    private readonly Transform ledgePoint;
    private readonly Transform getUpPosition;

    private int currentLockInputFrames;
    private int maxLockInputFrames = 600;

    bool onRightSide;

    public OnLedgeState(Transform ledgePoint, Transform getUpPosition)
    {
        this.ledgePoint = ledgePoint;
        this.getUpPosition = getUpPosition;
    }
    public void Enter(FighterController fighter)
    {
        currentLockInputFrames = 0;
        fighter.velocity = Vector3.zero;
        fighter.DebugColor(Color.orange);
        Vector3 pos = ledgePoint.position;
        pos.z = 0f;
        fighter.transform.position = pos;
    }

    public void Tick(FighterController fighter)
    {
        if (currentLockInputFrames <= maxLockInputFrames)
        {
            currentLockInputFrames++;
            return;
        }
        
        if (fighter.Input.JumpPressed)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.ChangeState(new AirborneState(fighter.facingRight));
            return;
        }

        else if (fighter.Input.FastFallPressed)
        {
            fighter.ChangeState(new AirborneState(fighter.facingRight));
            return;
        }

        else if (fighter.Input.BlockHeld)
        {
            //roll logic here (use lerp)
            fighter.ChangeState(new GroundedState());
            return;
        }
        //get up
        bool getUpPressed = onRightSide
            ? fighter.Controls.Player.Move.ReadValue<Vector2>().x < 0
            : fighter.Controls.Player.Move.ReadValue<Vector2>().x > 0;

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