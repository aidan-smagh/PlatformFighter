using UnityEngine;

public class SupineState : IFighterState
{
    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.yellow);

        fighter.transform.rotation = Quaternion.Euler(0, 0, 90f);

        Vector3 pos = fighter.transform.position;
        pos.y = 26.2f;
        fighter.transform.position = pos;
    }

    public void Tick(FighterController fighter)
    {
        
    }

    public void Exit(FighterController fighter)
    {
        
    }
}