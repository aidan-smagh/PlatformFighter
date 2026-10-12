using UnityEngine;

public interface IFighterInput
{
    Vector2 Move { get; }
    bool BlockHeld { get; }
    bool JumpPressed { get; }
    bool AttackPressed { get; }
    bool GrabPressed { get; }
    bool FastFallPressed { get; }
}