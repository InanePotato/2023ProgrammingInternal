using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerAttackScript : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManagerScript gameManagerScript;
    private KeyCode attackKeybind = KeyCode.Space;

    [Header("Player Attack Info")]
    public float attackRange;
    public float angleVariant;
    public GameObject targetEnemy;
    public float attackCooldown;
    private float attackCooldownTime;
    public PlayerStats playerStatsScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
        attackCooldownTime = 0;

        playerStatsScript = gameObject.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.gameOver || gameManagerScript.gamePaused)
        {
            return;
        }

        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies != null)
        {
            targetEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                GameObject enemyObject = enemy.transform.GetChild(0).gameObject;
                Vector2 targetDir = enemyObject.transform.position - gameObject.transform.position;
                float numDistanceToTarget = targetDir.magnitude;

                if (numDistanceToTarget <= attackRange)
                {
                    Vector2 forward = transform.right;
                    float angle = Vector2.Angle(targetDir, forward);
                    float checkAgainstAngle = 90;

                    if (FacingLeft())
                    {
                        // facing left
                        checkAgainstAngle -= angleVariant;

                        if (angle > checkAgainstAngle)
                        {
                            targetEnemy = enemy;
                            //Debug.Log("Can Attack at angle: " + angle);
                            break;
                        }
                    }
                    else
                    {
                        // facing right
                        checkAgainstAngle += angleVariant;

                        if (angle < checkAgainstAngle)
                        {
                            targetEnemy = enemy;
                            //Debug.Log("Can Attack at angle: " + angle);
                            break;
                        }
                    }
                }
            }

            if (Input.GetKeyDown(attackKeybind))// && Input.GetMouseButtonDown(0))
            {
                //IF there is an enemy to be attacked & no cooldown time left && left click is down
                if (targetEnemy != null && attackCooldownTime <= 0) // && Input.GetMouseButtonDown(0)
                {
                    // attack the target enemy
                    targetEnemy.GetComponent<EnemyScript>().DamageEnemy(playerStatsScript.damage);
                    //Debug.Log("Damaged enemy");

                    // once attacked, reset attack cooldown time
                    attackCooldownTime = attackCooldown;
                }
                else
                {
                    //Debug.LogWarning("Can't Attack: " + targetEnemy + ", " + attackCooldownTime);
                    // if still cooldown time left
                    if (attackCooldownTime > 0)
                    {
                        // take time off the cooldown time
                        attackCooldownTime -= Time.deltaTime;
                    }
                }
            } 
            
        }
    }

    public bool FacingLeft()
    {
        return gameManagerScript.playerSprite.GetComponent<SpriteRenderer>().flipX;
    }
}