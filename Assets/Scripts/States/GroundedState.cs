using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class GroundedState : IFighterState
{
    private enum LocomotionState { Idle, Walking, Running, TurnAround, None }
    private LocomotionState currentLocomotion = LocomotionState.Idle;

    private const float walkThreshold = 0.01f;
    private const float runThreshold = 0.5f;

    private bool isFacingRight = true;

    LocomotionState targetLocomotion;

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.green);
        currentLocomotion = LocomotionState.None;
        fc.fighter.moveSet.numJumps = 1;
    }

    public void Tick(FighterController fc)
    {
        fc.ApplyKnockbackDecay();

        float horizontalInput = Mathf.Abs(fc.HorizontalInput);

        bool turnedAround = fc.facingRight != isFacingRight;
        isFacingRight = fc.facingRight;

        if (turnedAround)
        {
            targetLocomotion = LocomotionState.TurnAround;
        }
        else if (horizontalInput <= walkThreshold)
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
            var overrideController = fc.animator.runtimeAnimatorController as AnimatorOverrideController;

            switch (currentLocomotion)
            {
                case LocomotionState.Idle:
                    overrideController["IdlePlaceholder"] = fc.idleAnimation;
                    fc.animator.Play("Idle", 0, 0f);
                    break;
                case LocomotionState.Walking:
                    overrideController["WalkPlaceholder"] = fc.walkAnimation;
                    fc.animator.Play("Walk", 0, 0f);
                    break;
                case LocomotionState.Running:
                    overrideController["RunPlaceholder"] = fc.runAnimation;
                    fc.animator.Play("Run", 0, 0f);
                    break;
                case LocomotionState.TurnAround:
                    fc.TurnedAround(isFacingRight);
                    break;
            }
        }

        HandleMove(fc);

        if (fc.velocity.y < 0)
        {
            fc.currentJumps = fc.fighter.moveSet.numJumps;
        }

        Vector3 knockback = new Vector3(fc.fighter.knockbackVelocity.x, fc.fighter.knockbackVelocity.y, 0f);
        fc.Controller.Move((fc.velocity + knockback) * Time.deltaTime);

        Vector2 dir = fc.Input.Move;

        if (fc.Input.JumpPressed)
        {
            fc.velocity.y = Mathf.Sqrt(fc.fighter.moveSet.jumpHeight * -2f * fc.fighter.gravity);
            fc.ChangeState(new AirborneState(isFacingRight));
            return;
        }

        if (fc.Input.AttackPressed)
        {
            MoveData forwardSmash = fc.fighter.moveSet.forwardSmash;
            fc.fighter.ClearHitTargets();
            var overrideController = fc.animator.runtimeAnimatorController as AnimatorOverrideController;
            overrideController["ForwardSmash"] = forwardSmash.animationClip;
            fc.animator.Play("Attack", 0, 0f);

            foreach (var hitbox in fc.swordHitboxes)
            {
                hitbox.Activate(forwardSmash);
            }
            return;
        }

        if (fc.Input.GrabPressed)
        {
            EnableGrabHitbox(fc);
            fc.StartCoroutine(DisableHitboxCoroutine(fc));
            return;
        }

        if (fc.Input.BlockHeld)
        {
            fc.ChangeState(new BlockState());
        }

        if (!fc.IsGrounded)
        {
            fc.ChangeState(new AirborneState(isFacingRight));
        }

        if (fc.Input.FastFallPressed && fc.TryGetPlatform(out Collider platform))
        {
            fc.DropThrough(platform);
        }
    }

    public void Exit(FighterController fc) { }

    void HandleMove(FighterController fc)
    {   
        fc.Controller.Move(new Vector3(fc.HorizontalInput, 0, 0) * fc.fighter.moveSet.runSpeed * Time.deltaTime);
    }

    public void EnableGrabHitbox(FighterController fc)
    {
        fc.grabHitbox.SetActive(true);
    }

    IEnumerator DisableHitboxCoroutine(FighterController fc)
    {
        yield return new WaitForSeconds(0.5f);
        fc.grabHitbox.SetActive(false);
    }


}