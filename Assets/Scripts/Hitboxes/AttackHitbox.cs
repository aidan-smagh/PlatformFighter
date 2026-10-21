using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Fighter fighter;
    MoveData move;
    Collider collider;

    void Awake()
    {
        collider = GetComponent<SphereCollider>();
        collider.enabled = false;
    }
    
    public void Activate(MoveData currentMove)
    {
        move = currentMove;
        collider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root) return;

        if (!other.CompareTag("Fighter")) return;
        
        Fighter target = other.gameObject.GetComponentInParent<Fighter>();

        if (target == null) return;
        if (fighter.HasAlreadyHit(target)) return;

        fighter.ReceiveHit(target, move);
        //fighter.CalculateMoveHit(target, move);
        //fighter.RegisterHit(target);
    }
}