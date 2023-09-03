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
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            return;
        }

        if (delayTimeCounter <= 0)
        {
            delayTimeCounter = UnityEngine.Random.Range(minDelayTime, maxDelayTime);
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
