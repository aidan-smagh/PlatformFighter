using UnityEngine;

public class GrabHitbox : MonoBehaviour
{
    FighterController dummy;
    FighterController fighter;

    void OnTriggerEnter(Collider other)
    {
        fighter = gameObject.GetComponentInParent<FighterController>();

        Vector3 grabberPos = gameObject.transform.position;
        dummy = other.GetComponentInParent<FighterController>();

        dummy.ChangeState(new GrabbedState(grabberPos));
        fighter.ChangeState(new GrabbingState());
    }
}