public interface IFighterState
{
    void Enter(FighterController fighter);
    void Tick(FighterController fighter);
    void Exit(FighterController fighter);
}