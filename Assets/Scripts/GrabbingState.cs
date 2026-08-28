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
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            //pummel
            EnablePummelHitbox(fighter);
            fighter.StartCoroutine(DisableHitboxCoroutine(fighter));
            return;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            //up throw
            dummy.CalculateMoveHit(dummy);
            fighter.ChangeState(new GroundedState());
            return;
        }
    }

    public void Exit(FighterController fighter)
    {
        
    }

    public void EnablePummelHitbox(FighterController fighter)
    {
        fighter.pummelHitbox.SetActive(true);
    }

    IEnumerator DisableHitboxCoroutine(FighterController fighter)
    {
        yield return new WaitForSeconds(0.5f);
        fighter.pummelHitbox.SetActive(false);
    }
}