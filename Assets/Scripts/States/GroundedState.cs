using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GroundedState : IFighterState
{
    private enum LocomotionState { Idle, Walking, Running }
    private LocomotionState currentLocomotion = LocomotionState.Idle;

    private const float walkThreshold = 0.01f;
    private const float runThreshold = 0.5f;

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.green);
    }

    public void Tick(FighterController fighter)
    {
        float horizontalInput = Mathf.Abs(fighter.HorizontalInput);

        LocomotionState targetLocomotion;

        if (horizontalInput <= walkThreshold)
        {
            targetLocomotion = LocomotionState.Idle;
        }
        else if (horizontalInput < runThreshold)
        {
            targetLocomotion = LocomotionState.Walking;
        }
        else
        {
            targetLocomotion = LocomotionState.Running;
        }

        if (targetLocomotion != currentLocomotion)
        {
            currentLocomotion = targetLocomotion;
            var overrideController = fighter.animator.runtimeAnimatorController as AnimatorOverrideController;

            switch (currentLocomotion)
            {
                case LocomotionState.Idle:
                    overrideController["IdlePlaceholder"] = fighter.idleAnimation;
                    fighter.animator.Play("Idle", 0, 0f);
                    break;
                case LocomotionState.Walking:
                    overrideController["WalkPlaceholder"] = fighter.walkAnimation;
                    fighter.animator.Play("Walk", 0, 0f);
                    break;
                case LocomotionState.Running:
                    overrideController["RunPlaceholder"] = fighter.runAnimation;
                    fighter.animator.Play("Run", 0, 0f);
                    break;
            }
        }

        HandleMove(fighter);

        if (fighter.velocity.y < 0)
        {
            fighter.currentJumps = fighter.Stats.numJumps;
        }

        Vector3 knockback = new Vector3(fighter.Stats.knockbackVelocity.x, fighter.Stats.knockbackVelocity.y, 0f);
        fighter.Controller.Move((fighter.velocity + knockback) * Time.deltaTime);

        if (fighter.isPlayerControlled)
        {
            Vector2 dir = fighter.Controls.Player.Move.ReadValue<Vector2>();
            
            //if (Keyboard.current.rKey.wasPressedThisFrame)
            if (fighter.Controls.Player.Shield.IsPressed() && dir.x > 0)
            {
                fighter.ChangeState(new RollState(1f));
                return;
            }

            //if (Keyboard.current.qKey.wasPressedThisFrame)
            if (fighter.Controls.Player.Shield.IsPressed() && dir.x < 0)
            {
                fighter.ChangeState(new RollState(-1f));
                return;
            }

            //if (Keyboard.current.spaceKey.wasPressedThisFrame)
            if (fighter.Controls.Player.Jump.WasPressedThisFrame())
            {
                fighter.velocity.y = Mathf.Sqrt(fighter.Stats.jumpHeight * -2f * fighter.Stats.gravity);
                fighter.ChangeState(new AirborneState());
                return;
            }

            //if (Keyboard.current.eKey.wasPressedThisFrame)
            if (fighter.Controls.Player.Attack.WasPressedThisFrame())
            {
                MoveData forwardSmash = fighter.moveSet.forwardSmash;
                fighter.Stats.ClearHitTargets();
                var overrideController = fighter.animator.runtimeAnimatorController as AnimatorOverrideController;
                overrideController["AttackPlaceholder"] = forwardSmash.animationClip;
                fighter.animator.Play("Attack", 0, 0f);

                foreach (var hitbox in fighter.swordHitboxes)
                {
                    hitbox.Activate(forwardSmash);
                }
                
                return;
            }

            //if (Keyboard.current.tKey.wasPressedThisFrame)
            if (fighter.Controls.Player.Grab.WasPressedThisFrame())
            {
                EnableGrabHitbox(fighter);
                fighter.StartCoroutine(DisableHitboxCoroutine(fighter));
                return;
            }

            //if (Keyboard.current.leftShiftKey.isPressed)
            if (fighter.Controls.Player.Shield.IsPressed())
            {
                fighter.isShieldActive = true;
            } else
            {
                fighter.isShieldActive = false;
            }
        }

        if (!fighter.IsGrounded())
        {
            fighter.ChangeState(new AirborneState());
        }
    }

    public void Exit(FighterController fighter) { }

    void HandleMove(FighterController fighter)
    {   
        fighter.Controller.Move(new Vector3(fighter.HorizontalInput, 0, 0) * fighter.Stats.runSpeed * Time.deltaTime);
    }

    public void EnableFSmashHitbox(FighterController fighter)
    {
        fighter.fSmashHitbox.SetActive(true);
    }

    public void EnableDSmashHitbox(FighterController fighter)
    {
        fighter.dSmashHitbox.SetActive(true);
    }

    public void EnableGrabHitbox(FighterController fighter)
    {
        fighter.grabHitbox.SetActive(true);
    }

    IEnumerator DisableHitboxCoroutine(FighterController fighter)
    {
        yield return new WaitForSeconds(0.5f);
        fighter.fSmashHitbox.SetActive(false);
        fighter.dSmashHitbox.SetActive(false);
        fighter.grabHitbox.SetActive(false);
    }


}