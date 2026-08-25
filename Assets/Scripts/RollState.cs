using UnityEngine;
using UnityEngine.InputSystem;

public class RollState : IFighterState
{

    private float rollDistance = 1f;
    private float rollSpeed = 0.1f;

    private readonly float direction;

    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsed;

    public RollState(float direction)
    {
        this.direction = direction;
    }

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.white);
        fighter.velocity = Vector3.zero;
        elapsed = 0f;

        //float direction = fighter.transform.position.x > 0 ? 1f : -1f;
        startPos = fighter.transform.position;
        endPos = startPos + Vector3.right * (direction * rollDistance);
    }

    public void Tick(FighterController fighter)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / rollSpeed);

        Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
        fighter.Controller.enabled = false;
        fighter.transform.position = newPos;
        fighter.Controller.enabled = true;

        if (t >= 1f)
        {
            fighter.ChangeState(new GroundedState());
        }
    }

    public void Exit(FighterController fighter) { }
}