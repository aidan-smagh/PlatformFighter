using UnityEngine;

public class ShieldManager : MonoBehaviour
{
    [SerializeField] CircleCollider2D collider;
    [SerializeField] private float decayRate = 0.0005f;
    [SerializeField] private float regenRate = 0.0005f;

    bool isActive;
    float maxRadiusSize = 1.1f;

    FighterController fighter;

    void Awake()
    {
        fighter = gameObject.GetComponentInParent<FighterController>();
    }

    void Update()
    {
        isActive = fighter.isShieldActive;
        
        if (isActive)
        {
            collider.radius -= decayRate;
        } 
        else if (!isActive && (collider.radius < maxRadiusSize))
        {
            collider.radius += regenRate;
        }
    }
}