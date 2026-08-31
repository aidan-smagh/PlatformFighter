using UnityEngine;
using UnityEngine.InputSystem;

public class GrabbedState : IFighterState
{

    int inputsNeeded;
    int currentInputs;

    float grabOffset = 1f;
    Vector3 grabberPos;

    public GrabbedState(Vector3 grabberPos)
    {
        this.grabberPos = grabberPos;
    }

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.pink);
        Vector3 pos = grabberPos;
        pos.x = grabberPos.x += grabOffset;
        fighter.transform.position = pos;
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