using UnityEngine;

public class GrabHitbox : MonoBehaviour
{
    //[SerializeField] FighterController fighter;
    //[SerializeField] DummyController dummy;
    FighterController fighter;

    void OnTriggerEnter(Collider other)
    {
        // fighter.grabbedFighter = other.GetComponent<Fighter>();
        // fighter.ChangeState(new GrabbingState());

        fighter = other.GetComponent<FighterController>();
        fighter.ChangeState(new GrabbedState());
    }
}