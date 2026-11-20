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

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.pink);
        Vector3 pos = grabberPos;
        pos.x = grabberPos.x += grabOffset;
        fc.transform.position = pos;
        inputsNeeded = Mathf.Max((int)fc.GetComponent<Fighter>().currentPercent, 5);
    }

    public void Tick(FighterController fc)
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            currentInputs += 1;
        }

        if (currentInputs >= inputsNeeded)
        {
            fc.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fc)
    {
        
    }
}