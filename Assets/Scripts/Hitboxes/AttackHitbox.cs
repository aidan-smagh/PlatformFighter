using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    Fighter dummy;
    
    void OnTriggerEnter(Collider other)
    {
        dummy = other.gameObject.GetComponent<Fighter>();
        fighter.CalculateMoveHit(dummy);
    }
}