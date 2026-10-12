using UnityEngine;

public class PlayerFighterInput : IFighterInput
{
    private readonly FighterControls controls;

    public PlayerFighterInput(FighterControls controls)
    {
        this.controls = controls;
    }

    public Vector2 Move => controls.Player.Move.ReadValue<Vector2>();
    public bool BlockHeld => controls.Player.Block.IsPressed();
    public bool JumpPressed => controls.Player.Jump.WasPressedThisFrame();
    public bool AttackPressed => controls.Player.Attack.WasPressedThisFrame();
    public bool GrabPressed => controls.Player.Grab.WasPressedThisFrame();
    public bool FastFallPressed => controls.Player.FastFall.IsPressed();
}