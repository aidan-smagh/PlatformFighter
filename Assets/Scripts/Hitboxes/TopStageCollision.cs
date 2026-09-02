using UnityEngine;

public class TopStageCollision : MonoBehaviour
{
    FighterController fighter;
    void OnTriggerEnter(Collider other)
    {
        //check fighters state
        //if tumble, switch to supine
        fighter = other.GetComponent<FighterController>();
        IFighterState state = fighter.currentState;
        if (state is TumbleState)
        {
            fighter.ChangeState(new SupineState());
        }
        //fighter.knockbackVelocity but its flipped to send in the opposite direction off ricochet
    }
}