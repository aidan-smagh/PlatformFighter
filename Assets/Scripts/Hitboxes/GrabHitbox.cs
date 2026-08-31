using UnityEngine;

public class GrabHitbox : MonoBehaviour
{
    FighterController fighter;

    void OnTriggerEnter(Collider other)
    {
        Vector3 grabberPos = gameObject.transform.position;
        fighter = other.GetComponent<FighterController>();
        fighter.ChangeState(new GrabbedState(grabberPos));
    }
}