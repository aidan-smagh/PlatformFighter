using UnityEngine;

public class LedgeContact : MonoBehaviour
{
    private GameObject fighter;
    GameManager gm;
    [SerializeField] GameObject fighterOnLedge;
    [SerializeField] Transform leftLedgeHang;
    [SerializeField] Transform rightLedgeHang;
    [SerializeField] Transform leftGetUpPosition;
    [SerializeField] Transform rightGetUpPosition;

    void OnTriggerEnter(Collider other)
    {
        if (!fighterOnLedge)
        {
            FighterController fighter = other.GetComponent<FighterController>();
            fighterOnLedge = other.gameObject;
            bool onRightSide = other.transform.position.x > 0;
            Transform transform = onRightSide ? rightLedgeHang : leftLedgeHang;
            Transform getUpPosition = onRightSide ? rightGetUpPosition : leftGetUpPosition;
            fighter.ChangeState(new OnLedgeState(transform, getUpPosition));
        }
        return;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == fighterOnLedge)
        {
            fighterOnLedge = null;
        }
    }
}
