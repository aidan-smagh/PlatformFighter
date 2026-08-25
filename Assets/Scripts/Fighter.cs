using UnityEngine;

public class Fighter : MonoBehaviour
{
    public float runSpeed = 5f;
    public float jumpHeight = 2.5f;
    public float fallMultiplier = 2.5f;
    public float gravity = -20f;
    public float numJumps = 1;
    public float weight = 100;

    [SerializeField] float currentPercent = 0f;
    [SerializeField] int stocks = 3;
    [SerializeField] GameObject spawnPoint;
    
    public int Stocks => stocks;
    GameObject fighter;

    Fighter dummy;

    void Awake()
    {
        fighter = gameObject;
    }

    public void RemoveStock()
    {
        stocks --;
        CharacterController cc = gameObject.GetComponent<CharacterController>();
        cc.enabled = false;
        fighter.transform.position = spawnPoint.transform.position;
        fighter.transform.rotation = spawnPoint.transform.rotation;
        cc.enabled = true;
    }

    void Knockback()
    {
        //figure out what to pass in here
        //fighter specific stats are already here
        //need opponent stats like specific move strength, rage
        //could be read from the hitbox hurtbox collision and then passed in
    }

    void CalculateMoveHit(Fighter other/*,hitbox move strength*/)
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        //compare tags in here eventually so theres no problems with false collisions
        dummy = other.gameObject.GetComponent<Fighter>();
        dummy.currentPercent = 15f;
    }
}
