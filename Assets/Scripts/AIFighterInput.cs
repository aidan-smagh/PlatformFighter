using UnityEngine;

public class AIFighterInput : IFighterInput
{
    public Vector2 Move { get; set; }
    public bool BlockHeld { get; set; }

    public bool JumpPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool GrabPressed { get; private set; }
    public bool FastFallPressed { get; private set; }
    public bool SpecialPressed { get; private set; }

    public void PressJump() => JumpPressed = true;
    public void PressAttack() => AttackPressed = true;
    public void PressGrab() => GrabPressed = true;
    public void PressFastFall() => FastFallPressed = true;
    public void PressSpecial() => SpecialPressed = true;

    public void ClearPressed()
    {
        JumpPressed = false;
        AttackPressed = false;
        GrabPressed = false;
        FastFallPressed = false;
        SpecialPressed = false;
    }
}