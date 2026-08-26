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

    public struct KnockbackData
    {
        public float d;
        public float s;
        public float b;
        public float r;
    }

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

    double CalculateKnockback(Fighter other, KnockbackData payload)
    {
        //figure out what to pass in here
        //fighter specific stats are already here
        //need opponent stats like specific move strength, rage
        //could be read from the hitbox hurtbox collision and then passed in
        //(((((p / 10 + pd / 20) * 200 / w + 100 * 1.4) + 18) * s) + b) * r
        
        double knockbackPower = ((((other.currentPercent / 10 + (other.currentPercent * payload.d) / 20) * (200 / (other.weight + 100) * 1.4) + 18) * payload.s) + payload.b) * payload.r;
        Debug.Log(knockbackPower);
        return knockbackPower;
    }

    void CalculateMoveHit(Fighter other/*,hitbox move strength*/)
    {
        //hard coded values for d, s, and b
        //ganon fsmash values as an example
        float d = 31f;
        float s = 0.75f;
        float b = 75f;
        float r = 1f;

        KnockbackData payload = new KnockbackData 
        { 
            d = d,
            s = s,
            b = b,
            r = r
        };

        CalculateKnockback(other, payload);
        other.currentPercent += d;
    }

    void OnTriggerEnter(Collider other)
    {
        //compare tags in here eventually so theres no problems with false collisions
        dummy = other.gameObject.GetComponent<Fighter>();
        CalculateMoveHit(dummy);
    }
}
