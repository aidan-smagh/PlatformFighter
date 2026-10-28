using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GrabbingState : IFighterState
{
    Fighter dummy;

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.purple);
        dummy = fighter.grabbedFighter;
    }

    public void Tick(FighterController fighter)
    {
        //if (Keyboard.current.eKey.wasPressedThisFrame)
        if (fighter.Controls.Player.Attack.WasPressedThisFrame())
        {
            //pummel
            return;
        }

        //will need to swap this out with a single function that will read Move input, apply its magnitude to a given threshold, and trigger a throw depending on which direction the Move stick is going
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            //up throw
            dummy.CalculateMoveHit(dummy, fighter.moveSet.upThrow, false);
            fighter.ChangeState(new GroundedState());
            return;
        }
    }

    public void Exit(FighterController fighter)
    {
        
    }
}