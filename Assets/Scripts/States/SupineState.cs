using UnityEngine;

public class SupineState : IFighterState
{
    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.yellow);
    }

    public void Tick(FighterController fighter)
    {
        
    }

    public void Exit(FighterController fighter)
    {
        
    }
}