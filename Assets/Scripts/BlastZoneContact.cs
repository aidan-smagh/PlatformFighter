using UnityEngine;

public class BlastZoneContact : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject fighter;
    GameManager gm;

    void OnTriggerEnter(Collider other)
    {
        Fighter fighter = other.GetComponent<Fighter>();
        fighter.RemoveStock();
    }
}
