using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    [SerializeField] MoveData move;
    Fighter dummy;
    
    void OnTriggerEnter(Collider other)
    {
        dummy = other.gameObject.GetComponentInParent<Fighter>();
        fighter.CalculateMoveHit(dummy, move);
    }
}