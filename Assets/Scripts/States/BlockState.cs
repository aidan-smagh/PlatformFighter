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

    public void Enter(FighterController fc)
    {
        fc.DebugColor(Color.teal);
        var overrideController = fc.animator.runtimeAnimatorController as AnimatorOverrideController;
        overrideController["BlockStartPlaceholder"] = fc.blockStartAnimation;
        overrideController["BlockHoldPlaceholder"] = fc.blockHoldAnimation;
        overrideController["BlockExitPlaceholder"] = fc.blockExitAnimation;

        SetPhase(fc, BlockPhase.Startup, "BlockStart");
    }

    public void Tick(FighterController fc)
    {
        phaseFrame++;

        switch(phase)
        {
            case BlockPhase.Startup:
                if (phaseFrame >= StartupFrames)
                {
                    if (fc.Controls.Player.Block.IsPressed())
                        SetPhase(fc, BlockPhase.Hold, "BlockHold");
                    else
                        SetPhase(fc, BlockPhase.Exit, "BlockExit");
                }
                break;
            
            case BlockPhase.Hold:
                currentBlockFrame++;
                if (!fc.Controls.Player.Block.IsPressed())
                    SetPhase(fc, BlockPhase.Exit, "BlockExit");
                break;

            case BlockPhase.Exit:
                if (phaseFrame >= ExitFrames)
                    fc.ChangeState(new GroundedState());
                break;
        }        
    }

    public void Exit(FighterController fc)
    {
    }

    private void SetPhase(FighterController fc, BlockPhase newPhase, string animState)
    {
        phase = newPhase;
        phaseFrame = 0;
        fc.animator.Play(animState, 0, 0f);
    }
}