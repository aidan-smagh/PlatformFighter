using UnityEngine;

public class GrabHitbox : MonoBehaviour
{
    FighterController dummy;
    FighterController fighter;

    GameObject grabber;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root) return;

        fighter = gameObject.GetComponentInParent<FighterController>();

        Vector3 grabberPos = gameObject.transform.parent.position;
        dummy = other.GetComponentInParent<FighterController>();

        dummy.ChangeState(new GrabbedState(grabberPos));
        fighter.ChangeState(new GrabbingState());
    }
}