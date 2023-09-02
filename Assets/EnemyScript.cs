using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.Progress;

public class EnemyScript : MonoBehaviour
{
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;
    private GameObject canvas;

    [Header("Enemy Details")]
    public float maxHealth;
    private float health;
    public GameObject healthBarPrefab;
    private GameObject healthBar;
    public float HealthBarYPosOffset;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    private float attackCooldownCount;
    private bool canAttack;
    private bool inAttackRange;
    private bool touchingPlayer;
    public AttackType attackType;
    private SpriteRenderer sprite;
    public GameObject enemy;
    public GameObject enemySpawner;
    public int killScore;
    public UnityEngine.Color scoreAdditionPopupColour;

    public enum AttackType { Melee, Spell, Range };

    [Header("Movement")]
    public MovmentType movmentType;
    public float speed = 1.0f;
    private Rigidbody2D rigidbody;
    public Vector2 movementOffset;
    private float changeWanderCooldown;
    private Vector2 wanderTarget;
    private Vector2 linearTarget;
    public bool spriteFlipped;

    public enum MovmentType { Raom, Linear };

    [Header("Player Targeting")]
    public bool canTarget;
    public float targetingRange;
    private GameObject target;
    private bool playerInRange;
    private Vector2 directionToTarget;
    private float numDistanceToTarget;

    [Header("Animations")]
    private EnemyAnimationScript animationScript;

    [Header("Loot")]
    public float lootScatter;
    public List<LootDrop> lootDrops = new List<LootDrop>();
    private GameObject droppedItemPrefab;
    public LootTable lootTable;
    public int minItemsFromLootTable;
    public int maxItemsFromLootTable;

    [Serializable]
    public struct LootDrop
    {
        public Item item;
        public int minAmmount;
        public int maxAmmount;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        canvas = gameManagerScript.mainCanvis;

        enemy = gameObject.transform.GetChild(0).gameObject;
        sprite = enemy.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        //animator = enemy.transform.GetChild(0).gameObject.GetComponent<Animator>();
        animationScript = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyAnimationScript>();
        rigidbody = enemy.GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectsWithTag("Player").First();

        Vector2 linearPoint = new Vector2(transform.position.x + movementOffset.x, transform.position.y + movementOffset.y);
        Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
        linearTarget = distanceToLinearTarget.normalized;

        attackCooldownCount = 0;

        health = maxHealth;

        droppedItemPrefab = gameManagerScript.droppedItemPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        // if game paused or over, do nothing
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            return;
        }

        // get closest disance
        Vector2 distanceToRight = target.transform.Find("PlayerRightPosition").gameObject.transform.position - enemy.transform.position;
        Vector2 distanceToMiddle = target.transform.position - enemy.transform.position;
        Vector2 distanceToLeft = target.transform.Find("PlayerLeftPosition").position - enemy.transform.position;

        Vector2 directionToRight = distanceToRight.normalized;
        Vector2 directionToMiddle = distanceToMiddle.normalized;
        Vector2 directionToLeft = distanceToLeft.normalized;

        float numDistanceToRight = distanceToRight.magnitude;
        float numDistanceToMiddle = distanceToMiddle.magnitude;
        float numDistanceToLeft = distanceToLeft.magnitude;

        if (numDistanceToRight <= numDistanceToMiddle && numDistanceToRight <= numDistanceToLeft)
        {
            directionToTarget = directionToRight;
            numDistanceToTarget = numDistanceToRight;
        }
        else if (numDistanceToLeft <= numDistanceToMiddle && numDistanceToLeft <= numDistanceToRight)
        {
            directionToTarget = directionToLeft;
            numDistanceToTarget = numDistanceToLeft;
        }
        else
        {
            directionToTarget = directionToMiddle;
            numDistanceToTarget = numDistanceToMiddle;
        }

        // if the target is in targeting range
        if (numDistanceToTarget <= targetingRange && canTarget && numDistanceToTarget > attackRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        // if the target is in attack range
        if (numDistanceToTarget <= attackRange)
        {
            inAttackRange = true;
            rigidbody.velocity = new Vector2(0, 0);
        }
        else
        {
            inAttackRange = false;
        }

        if (inAttackRange || touchingPlayer)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }

        // if roaming
        if (movmentType == MovmentType.Raom)
        {
            if (changeWanderCooldown <= 0)
            {
                float randomX = UnityEngine.Random.Range(-movementOffset.x, movementOffset.x);
                float randomY = UnityEngine.Random.Range(-movementOffset.y, movementOffset.y);
                Vector2 wanderPoint = new Vector2(transform.position.x + randomX, transform.position.y + randomY);

                Vector2 distanceToWanderTarget = ToEnemiesVector3(wanderPoint) - enemy.transform.position;

                wanderTarget = distanceToWanderTarget.normalized;

                changeWanderCooldown = UnityEngine.Random.Range(1f, 3f);
            }
            else
            {
                changeWanderCooldown -= Time.deltaTime;
            }
        }
        else if (movmentType == MovmentType.Linear)
        {
            if (enemy.transform.position.x >= transform.position.x + movementOffset.x && enemy.transform.position.y >= transform.position.y + movementOffset.y)
            {
                Vector2 linearPoint = new Vector2(transform.position.x - movementOffset.x, transform.position.y - movementOffset.y);
                Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
                linearTarget = distanceToLinearTarget.normalized;
            }
            else if (enemy.transform.position.x <= transform.position.x - movementOffset.x && enemy.transform.position.y <= transform.position.y - movementOffset.y)
            {
                Vector2 linearPoint = new Vector2(transform.position.x + movementOffset.x, transform.position.y + movementOffset.y);
                Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
                linearTarget = distanceToLinearTarget.normalized;
            }
        }

        // attack player
        if (canAttack && attackCooldownCount <= 0)
        {
            attackCooldownCount = attackCooldown;
            animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Attack);

            if (attackType == AttackType.Range)
            {
                RangeAttackPlayer();
            }
            else if (attackType == AttackType.Spell)
            {
                SpellAttackPlayer();
            }
            else
            {
                MeleeAttackPlayer();
            }
        }

        if (attackCooldownCount > 0)
        {
            attackCooldownCount -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // if game paused or over, do nothing
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            return;
        }

        if (!canAttack)
        {
            animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Walk);

            if (playerInRange && canTarget)
            {
                // move to player target
                rigidbody.velocity = new Vector2(directionToTarget.x, directionToTarget.y) * speed;
            }
            else
            {
                if (movmentType == MovmentType.Raom)
                {
                    // Move to wander target
                    rigidbody.velocity = new Vector2(wanderTarget.x, wanderTarget.y) * speed;
                }
                else if (movmentType == MovmentType.Linear)
                {
                    // Move linear target
                    rigidbody.velocity = new Vector2(linearTarget.x, linearTarget.y) * speed;
                }
            }
        }
        
        if (rigidbody.velocity.x > 0)
        {
            sprite.flipX = spriteFlipped;
        }
        else if (rigidbody.velocity.x < 0)
        {
            sprite.flipX = !spriteFlipped;
        }
    }

    Vector3 ToEnemiesVector3 (Vector2 initial)
    {
        return new Vector3(initial.x, initial.y, enemy.transform.position.z);
    }

    public void DamageEnemy(float damage)
    {
        health -= damage;
        animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Damaged);

        if (health <= 0)
        {
            // drop any special loot drops
            foreach (LootDrop item in lootDrops)
            {
                int ammount = UnityEngine.Random.Range(item.minAmmount, item.maxAmmount);
                DropItem(item.item, ammount);
            }

            // prepare for loot table loot drops
            List<int> LootTableItemIDS = new List<int>();
            for (int id = 0; id < lootTable.lootTable.Count(); id++)
            {
                for (int i = 0; i < lootTable.lootTable[id].rarity; i++)
                {
                    LootTableItemIDS.Add(id);
                }
            }

            // drop loot table loot
            int numItemsFromLootTable = UnityEngine.Random.Range(minItemsFromLootTable, maxItemsFromLootTable);
            for (int i = 0; i < numItemsFromLootTable; i++)
            {
                //get a random id to drop
                int randomID = LootTableItemIDS[UnityEngine.Random.Range(0, LootTableItemIDS.Count() - 1)];
                Debug.Log(randomID.ToString());
                Debug.Log(lootTable.lootTable[randomID].minAmmountPerDrop.ToString() + " - " + lootTable.lootTable[randomID].maxAmmountPerDrop.ToString());

                // get ammount
                int ammount = UnityEngine.Random.Range(lootTable.lootTable[randomID].minAmmountPerDrop, lootTable.lootTable[randomID].maxAmmountPerDrop);
                // get Item
                Item item = lootTable.lootTable[randomID].item;

                DropItem(item, ammount);
            }

            target.GetComponent<PlayerStats>().ChangeScore(killScore);
            gameManagerScript.DisplayMessage(killScore.ToString(), gameObject, scoreAdditionPopupColour);

            // IF this enemy is attached to a spawner
            if (enemySpawner != null)
            {
                // remove this enemy from the spawners list
                enemySpawner.GetComponent<SpawnerScript>().spawnedEnemies.Remove(gameObject);
            }

            // destroy enemy and health bar
            Destroy(healthBar);
            Destroy(gameObject);
            return;
        }

        // there is no current health bar for the enemy
        if (healthBar == null)
        {
            GameObject newHealthBar = Instantiate(healthBarPrefab, canvas.transform);
            EnemyHealthBarScript newHealthBarScript = newHealthBar.GetComponent<EnemyHealthBarScript>();

            newHealthBarScript.maxHealth = maxHealth;
            newHealthBarScript.currentHealth = health;
            newHealthBarScript.enemyObject = gameObject.transform.Find("Enemy").gameObject;
            newHealthBarScript.yPosOffSet = HealthBarYPosOffset;
            
            healthBar = newHealthBar;
        }
        else
        {
            healthBar.GetComponent<EnemyHealthBarScript>().DecreaseEnemyHealthBar(damage);
        }
    }

    private void DropItem(Item item, int ammount)
    {
        GameObject newItemDrop = Instantiate(droppedItemPrefab);

        Vector2 enemyPosition = gameObject.transform.GetChild(0).position;

        float x = UnityEngine.Random.Range(enemyPosition.x + lootScatter, enemyPosition.x - lootScatter);
        float y = UnityEngine.Random.Range(enemyPosition.y + lootScatter, enemyPosition.y - lootScatter);
        newItemDrop.transform.position = new Vector2(x, y);

        newItemDrop.GetComponent<ItemPickup>().item = item;
        newItemDrop.GetComponent<ItemPickup>().ammount = ammount;
    }

    private void MeleeAttackPlayer()
    {
        target.GetComponent<PlayerStats>().SubtractHealth(damage);
    }

    private void RangeAttackPlayer()
    {

    }

    private void SpellAttackPlayer()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == target)
        {
            touchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == target)
        {
            touchingPlayer = false;
        }
    }
}
