using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbedState : IFighterState
{

    int inputsNeeded;
    int currentInputs;

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.pink);
        inputsNeeded = (int)fighter.GetComponent<Fighter>().currentPercent;
    }

    public void Tick(FighterController fighter)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            currentInputs += 1;
        }

        if (currentInputs >= inputsNeeded)
        {
            fighter.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fighter)
    {
        
    }
}