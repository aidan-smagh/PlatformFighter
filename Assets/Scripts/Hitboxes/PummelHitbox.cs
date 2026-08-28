using UnityEngine;

public class PummelHitbox : MonoBehaviour
{
    Fighter dummy;

    void OnTriggerEnter(Collider other)
    {
        //change the grabbed fighter's tag to something specific so only they are hit by this hitbox
        //wont actually matter if the game stays as 1v1s but if i want to add more fighters at once this is a good change
        dummy = other.gameObject.GetComponent<Fighter>();
        
        //use this whenever each attack gets organized into SOs
        //fighter.CalculateMoveHit(dummy);

        dummy.currentPercent += 1f;
    }
}