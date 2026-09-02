using UnityEngine;

public class TumbleState : IFighterState
{
    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.black);
    }

    public void Tick(FighterController fighter)
    {
        //rotate the character

    }

    public void Exit(FighterController fighter)
    {
        
    }
}