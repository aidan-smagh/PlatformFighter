using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveset", menuName = "Fighter/Moveset")]
public class MovesetData : ScriptableObject
{
    public MoveData jab;

    public MoveData forwardSmash;
    public MoveData downSmash;
    public MoveData upSmash;

    public MoveData forwardTilt;
    public MoveData downTilt;
    public MoveData upTilt;

    public MoveData upThrow;
    public MoveData downThrow;
    public MoveData forwardThrow;
    public MoveData backThrow;

    public MoveData pummel;

    public MoveData dashAttack;

    //implement specials later if wanted
}