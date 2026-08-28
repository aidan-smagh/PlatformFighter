using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GroundedState : IFighterState
{
    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.green);
    }

    public void Tick(FighterController fighter)
    {
        HandleMove(fighter);

        if (fighter.velocity.y < 0)
        {
            fighter.currentJumps = fighter.Stats.numJumps;
        }

        fighter.Controller.Move(fighter.velocity * Time.deltaTime);

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            fighter.ChangeState(new RollState(1f));
            return;
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            fighter.ChangeState(new RollState(-1f));
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
            fighter.ChangeState(new AirborneState());
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            EnableTestHitbox(fighter);
            fighter.StartCoroutine(DisableHitboxCoroutine(fighter));
            return;
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            EnableGrabHitbox(fighter);
            fighter.StartCoroutine(DisableHitboxCoroutine(fighter));
            return;
        }

        if (!fighter.IsGrounded())
        {
            fighter.ChangeState(new AirborneState());
        }
    }

    public void Exit(FighterController fighter) { }

    void HandleMove(FighterController fighter)
    {
        float h = 0f;
        if (Keyboard.current.aKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed) h = 1f;
        fighter.Controller.Move(new Vector3(h, 0, 0) * fighter.Stats.runSpeed * Time.deltaTime);
    }

    public void EnableTestHitbox(FighterController fighter)
    {
        fighter.testHitbox.SetActive(true);
    }

    public void EnableGrabHitbox(FighterController fighter)
    {
        fighter.grabHitbox.SetActive(true);
    }

    IEnumerator DisableHitboxCoroutine(FighterController fighter)
    {
        yield return new WaitForSeconds(0.5f);
        fighter.testHitbox.SetActive(false);
        fighter.grabHitbox.SetActive(false);
    }


}