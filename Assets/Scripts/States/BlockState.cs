using UnityEngine;

public class BlockState : IFighterState
{

    private enum BlockPhase { Startup, Hold, Exit }

    private const int StartupFrames = 4;
    private const int ExitFrames = 6;

    public const int ParryStart = 0;
    public const int ParryEnd = 1000;
    public int currentBlockFrame = 0;

    private BlockPhase phase;
    private int phaseFrame;

    public void Enter(FighterController fighter)
    {
        fighter.DebugColor(Color.teal);
        var overrideController = fighter.animator.runtimeAnimatorController as AnimatorOverrideController;
        overrideController["BlockStartPlaceholder"] = fighter.blockStartAnimation;
        overrideController["BlockHoldPlaceholder"] = fighter.blockHoldAnimation;
        overrideController["BlockExitPlaceholder"] = fighter.blockExitAnimation;

        SetPhase(fighter, BlockPhase.Startup, "BlockStart");
    }

    public void Tick(FighterController fighter)
    {
        phaseFrame++;

        switch(phase)
        {
            case BlockPhase.Startup:
                if (phaseFrame >= StartupFrames)
                {
                    if (fighter.Controls.Player.Block.IsPressed())
                        SetPhase(fighter, BlockPhase.Hold, "BlockHold");
                    else
                        SetPhase(fighter, BlockPhase.Exit, "BlockExit");
                }
                break;
            
            case BlockPhase.Hold:
                currentBlockFrame++;
                if (!fighter.Controls.Player.Block.IsPressed())
                    SetPhase(fighter, BlockPhase.Exit, "BlockExit");
                break;

            case BlockPhase.Exit:
                if (phaseFrame >= ExitFrames)
                    fighter.ChangeState(new GroundedState());
                break;
        }        
    }

    public void Exit(FighterController fighter)
    {
    }

    private void SetPhase(FighterController fighter, BlockPhase newPhase, string animState)
    {
        phase = newPhase;
        phaseFrame = 0;
        fighter.animator.Play(animState, 0, 0f);
    }
}