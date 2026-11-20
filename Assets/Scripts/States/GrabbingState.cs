using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GrabbingState : IFighterState
{
    Fighter dummy;

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.purple);
        dummy = fc.grabbedFighter;
    }

    public void Tick(FighterController fc)
    {
        if (fc.Controls.Player.Attack.WasPressedThisFrame())
        {
            //pummel
            return;
        }

        //will need to swap this out with a single function that will read Move input, apply its magnitude to a given threshold, and trigger a throw depending on which direction the Move stick is going
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            //up throw
            dummy.CalculateMoveHit(dummy, fc.fighter.moveSet.upThrow, false);
            fc.ChangeState(new GroundedState());
            return;
        }
    }

    public void Exit(FighterController fc)
    {
        
    }
}