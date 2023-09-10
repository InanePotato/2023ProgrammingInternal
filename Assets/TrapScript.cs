using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript;

    public bool active;
    public bool canAttackPlayer;
    public float minDelayTime;
    public float maxDelayTime;
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
        delayTimeCounter = UnityEngine.Random.Range(minDelayTime, maxDelayTime);

        playerObject = gameManagerScript.player;
    }

    // Update is called once per frame
    void Update()
    {
        // IF paused or game over
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            // don;t continue
            return;
        }

        // IF timer cooldown up
        if (delayTimeCounter <= 0)
        {
            // reset cooldown
            delayTimeCounter = UnityEngine.Random.Range(minDelayTime, maxDelayTime);
            active = !active;
            
            // IF now active
            if (active)
            {
                // play activate animation
                animator.Play("activateSpikes");
            }
            else
            {
                // ELSE playe deactivate animation
                animator.Play("deactivateSpikes");
            }
        }
        else
        {
            // ELSE decrease timer cooldown
            delayTimeCounter -= Time.deltaTime;
        }

        // IF can actually damage the player
        if (active && canAttackPlayer)
        {
            // calculate damage based on damage per second
            float damage = Time.deltaTime * damagePerSecond;
            // damage player
            gameManagerScript.player.GetComponent<PlayerStats>().SubtractHealth(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // IF collided with player
        if (collision.gameObject == playerObject)
        {
            // set can attack to true
            canAttackPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // IF no longer collided with player
        if (collision.gameObject == playerObject)
        {
            // set can attack to false
            canAttackPlayer = false;
        }
    }
}
