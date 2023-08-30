using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript;

    public bool active;
    public bool canAttackPlayer;
    public float delayTime;
    public float damagePerSecond;
    [SerializeField]
    private float delayTimeCounter;
    private Animator animator;
    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        animator = gameObject.GetComponent<Animator>();

        canAttackPlayer = false;
        active = false;
        delayTimeCounter = delayTime;

        playerObject = gameManagerScript.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            return;
        }

        if (delayTimeCounter <= 0)
        {
            delayTimeCounter = delayTime;
            active = !active;
            
            if (active)
            {
                animator.Play("activateSpikes");
            }
            else
            {
                animator.Play("deactivateSpikes");
            }
        }
        else
        {
            delayTimeCounter -= Time.deltaTime;
        }

        if (active && canAttackPlayer)
        {
            float damage = Time.deltaTime * damagePerSecond;
            gameManagerScript.player.GetComponent<PlayerStats>().SubtractHealth(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            canAttackPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == playerObject)
        {
            canAttackPlayer = false;
        }
    }
}
