using UnityEngine;

public class SupineState : IFighterState
{
    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.yellow);

        fc.transform.rotation = Quaternion.Euler(0, 0, 90f);

        Vector3 pos = fc.transform.position;
        pos.y = 26.2f;
        fc.transform.position = pos;
    }

    public void Tick(FighterController fc)
    {
        
    }

    public void Exit(FighterController fc)
    {
        
    }
}