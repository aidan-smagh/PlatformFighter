using UnityEngine;

public class AIController : MonoBehaviour
{
    public AIFighterInput aiInput;
    public FighterController fighter;

    float attackTimer;

    void Awake()
    {
        fighter = GetComponent<FighterController>();
    }

    void Start()
    {
        aiInput = fighter.Input as AIFighterInput;

        if (aiInput != null)
            aiInput.Move = new Vector2(-0.3f, 0f);
    }

    void Update()
    {
        aiInput.ClearPressed();

        attackTimer += Time.deltaTime;

        if (attackTimer >= 2f)
        {
            Debug.Log("attack");
            aiInput.PressAttack();
            attackTimer = 0f;
        }
    }
}