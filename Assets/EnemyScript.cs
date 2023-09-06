using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Declare class scope variables for game manager references
    [Header("Game Manager")]
    private GameManagerScript gameManagerScript;
    private GameObject canvas;

    // Declare class scope variables for enemy details (eg. stats)
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

    // Declare class scope variables for movement
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

    // Declare class scope variables for targeting
    [Header("Player Targeting")]
    public bool canTarget;
    public float targetingRange;
    private GameObject target;
    private bool playerInRange;
    private Vector2 directionToTarget;
    private float numDistanceToTarget;

    [Header("Animations")]
    private EnemyAnimationScript animationScript;

    // Declare class scope variables for loot drops
    [Header("Loot")]
    public float lootScatter;
    public List<LootDrop> lootDrops = new List<LootDrop>();
    private GameObject droppedItemPrefab;
    public LootTable lootTable;
    public int minItemsFromLootTable;
    public int maxItemsFromLootTable;

    // Declare Serializable structure for loot drops
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
        // Get and set a reference to the game manager script
        gameManagerScript = GameManagerScript.Instance;

        // Get and set a reference to the main UI canvas
        canvas = gameManagerScript.mainCanvis;

        // Get and set a references for enemy, sprite, animator, and rigidbody
        enemy = gameObject.transform.GetChild(0).gameObject;
        sprite = enemy.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        animationScript = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyAnimationScript>();
        rigidbody = enemy.GetComponent<Rigidbody2D>();

        // Get and set a reference to the player (target)
        target = GameObject.FindGameObjectsWithTag("Player").First();

        // Set location for linear movement target
        Vector2 linearPoint = new Vector2(transform.position.x + movementOffset.x, transform.position.y + movementOffset.y);
        Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
        linearTarget = distanceToLinearTarget.normalized;

        // Default attack cooldown to 0
        attackCooldownCount = 0;

        // Default health to max health
        health = maxHealth;

        // Get and set a reference to the dropped item prefab
        droppedItemPrefab = gameManagerScript.droppedItemPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        // IF game is paused or over
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            // Don’t continue (return)
            return;
        }

        // Figure out what point on the enemy is the closest (right, middle, left)
        // Get difference in location of each major point of the target (left, middle, right)
        Vector2 distanceToRight = target.transform.Find("PlayerRightPosition").gameObject.transform.position - enemy.transform.position;
        Vector2 distanceToMiddle = target.transform.position - enemy.transform.position;
        Vector2 distanceToLeft = target.transform.Find("PlayerLeftPosition").position - enemy.transform.position;

        // Get direction to each major point of the target
        Vector2 directionToRight = distanceToRight.normalized;
        Vector2 directionToMiddle = distanceToMiddle.normalized;
        Vector2 directionToLeft = distanceToLeft.normalized;

        // Get distance to each major point of the target
        float numDistanceToRight = distanceToRight.magnitude;
        float numDistanceToMiddle = distanceToMiddle.magnitude;
        float numDistanceToLeft = distanceToLeft.magnitude;

        // IF right distance is closest
        if (numDistanceToRight <= numDistanceToMiddle && numDistanceToRight <= numDistanceToLeft)
        {
            // Set direction and distance to target to this
            directionToTarget = directionToRight;
            numDistanceToTarget = numDistanceToRight;
        }
        else if (numDistanceToLeft <= numDistanceToMiddle && numDistanceToLeft <= numDistanceToRight)
        {
            // ELSE IF left distance is closest
            // Set direction and distance to target to this
            directionToTarget = directionToLeft;
            numDistanceToTarget = numDistanceToLeft;
        }
        else
        {
            // Set direction and distance to target to middle distance & direction
            directionToTarget = directionToMiddle;
            numDistanceToTarget = numDistanceToMiddle;
        }

        // IF distance to target is in enemy targeting range
        if (numDistanceToTarget <= targetingRange && canTarget && numDistanceToTarget > attackRange)
        {
            // Set in target range to true
            playerInRange = true;
        }
        else
        {
            // Set in target range to false
            playerInRange = false;
        }

        // IF distance to target is in enemy attack range
        if (numDistanceToTarget <= attackRange)
        {
            // Set in attack range to true
            inAttackRange = true;
            // Set enemy velocity to 0 in all directions
            rigidbody.velocity = new Vector2(0, 0);
        }
        else
        {
            // Set in attack range to false
            inAttackRange = false;
        }

        // IF in attack range or touching player
        if (inAttackRange || touchingPlayer)
        {
            // Set can attack to true
            canAttack = true;
        }
        else
        {
            // Set can attack to false
            canAttack = false;
        }

        // IF enemy movement type is ‘roaming’
        if (movmentType == MovmentType.Raom)
        {
            // IF should change roam target
            if (changeWanderCooldown <= 0)
            {
                // Get new random X & Y target position based off roam area
                float randomX = UnityEngine.Random.Range(-movementOffset.x, movementOffset.x);
                float randomY = UnityEngine.Random.Range(-movementOffset.y, movementOffset.y);
                Vector2 wanderPoint = new Vector2(transform.position.x + randomX, transform.position.y + randomY);

                // Calculate difference in locations of wander target
                Vector2 distanceToWanderTarget = ToEnemiesVector3(wanderPoint) - enemy.transform.position;

                // Calculate distance to wander target
                wanderTarget = distanceToWanderTarget.normalized;

                // Randomly set new direction change cooldown
                changeWanderCooldown = UnityEngine.Random.Range(1f, 3f);
            }
            else
            {
                // Decrease change direction cooldown
                changeWanderCooldown -= Time.deltaTime;
            }
        }
        else if (movmentType == MovmentType.Linear)
        {
            // ELSE IF enemy movement type is ‘linear’
            // IF enemy is at right linear point
            if (enemy.transform.position.x >= transform.position.x + movementOffset.x && enemy.transform.position.y >= transform.position.y + movementOffset.y)
            {
                // Get left linear point
                Vector2 linearPoint = new Vector2(transform.position.x - movementOffset.x, transform.position.y - movementOffset.y);
                // Get difference in location to left linear point
                Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
                // Get distance to left linear point
                linearTarget = distanceToLinearTarget.normalized;
            }
            else if (enemy.transform.position.x <= transform.position.x - movementOffset.x && enemy.transform.position.y <= transform.position.y - movementOffset.y)
            {
                // ELSE IF enemy is at left linear point
                // Get right linear point
                Vector2 linearPoint = new Vector2(transform.position.x + movementOffset.x, transform.position.y + movementOffset.y);
                // Get difference in location to right linear point
                Vector2 distanceToLinearTarget = ToEnemiesVector3(linearPoint) - enemy.transform.position;
                // Get distance to right linear point
                linearTarget = distanceToLinearTarget.normalized;
            }
        }

        // IF player can attack and no cooldown left
        if (canAttack && attackCooldownCount <= 0)
        {
            // Reset attack cooldown
            attackCooldownCount = attackCooldown;
            // Change to attack animation state (using animation scripts ChangeState method)
            animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Attack);

            // IF attack type is ‘range’
            if (attackType == AttackType.Range)
            {
                // Call on RangeAttack method
                RangeAttackPlayer();
            }
            else if (attackType == AttackType.Spell)
            {
                // ELSE IF attack type is ‘spell’
                // Call on SpellAttack method
                SpellAttackPlayer();
            }
            else
            {
                // Call on MeleeAttack method
                MeleeAttackPlayer();
            }
        }

        // IF attack cooldown isn’t 0
        if (attackCooldownCount > 0)
        {
            // Decrease attack cooldown
            attackCooldownCount -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // IF game paused or over
        if (gameManagerScript.gamePaused || gameManagerScript.gameOver)
        {
            // Don’t continue (return)
            return;
        }

        // IF can’t attack (can move)
        if (!canAttack)
        {
            // Change to move animation state (using animation scripts ChangeState method)
            animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Walk);

            // IF player is player is in target range and enemy is allowed to target
            if (playerInRange && canTarget)
            {
                // Set velocity to move enemy towards target
                rigidbody.velocity = new Vector2(directionToTarget.x, directionToTarget.y) * speed;
            }
            else
            {
                // IF movement type is ‘roam’
                if (movmentType == MovmentType.Raom)
                {
                    // Set velocity to move enemy towards roam target
                    rigidbody.velocity = new Vector2(wanderTarget.x, wanderTarget.y) * speed;
                }
                else if (movmentType == MovmentType.Linear)
                {
                    // ELSE IF movement type is ‘linear’
                    // Set velocity to move enemy towards linear target
                    rigidbody.velocity = new Vector2(linearTarget.x, linearTarget.y) * speed;
                }
            }
        }

        // IF enemy is moving right
        if (rigidbody.velocity.x > 0)
        {
            // Flip the sprite so it’s facing right
            sprite.flipX = spriteFlipped;
        }
        else if (rigidbody.velocity.x < 0)
        {
            // ELSE IF enemy is moving left
            // Flip the sprite so it’s facing left
            sprite.flipX = !spriteFlipped;
        }
    }

    /// <summary>
    /// Used to change a vector 2 into a vector 3 relevent to enemy current z location
    /// </summary>
    /// <param name="initial"></param>
    /// <returns></returns>
    Vector3 ToEnemiesVector3 (Vector2 initial)
    {
        // Return a value with the inputs x & y but current enemy z location
        return new Vector3(initial.x, initial.y, enemy.transform.position.z);
    }

    /// <summary>
    /// Handles damaging of the enemy and diaplay of enemy health
    /// </summary>
    /// <param name="damage"></param>
    public void DamageEnemy(float damage)
    {
        // Decrease enemy health by damage
        health -= damage;
        // Play damaged animation
        animationScript.ChangeAnimationState(EnemyAnimationScript.AnimationState.Damaged);

        // IF health is 0 or less (enemy dead)
        if (health <= 0)
        {
            // FOREACH specified drop item
            foreach (LootDrop item in lootDrops)
            {
                // Set a random amount to drop using min and max values
                int ammount = UnityEngine.Random.Range(item.minAmmount, item.maxAmmount);
                // Call on the DropItem method with the item and amount
                DropItem(item.item, ammount);
            }

            // Declare list for loot table ID’s based on rarity
            List<int> LootTableItemIDS = new List<int>();
            // FOR amount of items in loot table
            for (int id = 0; id < lootTable.lootTable.Count(); id++)
            {
                // FOR rarity value of items in loot table
                for (int i = 0; i < lootTable.lootTable[id].rarity; i++)
                {
                    // Add item loot table ID to list
                    LootTableItemIDS.Add(id);
                }
            }

            // Set a random number of drops based on min and max values
            int numItemsFromLootTable = UnityEngine.Random.Range(minItemsFromLootTable, maxItemsFromLootTable);
            // FOR number of drops
            for (int i = 0; i < numItemsFromLootTable; i++)
            {
                // Pick a random ID from the ID’s list
                int randomID = LootTableItemIDS[UnityEngine.Random.Range(0, LootTableItemIDS.Count() - 1)];

                // Set a random amount of the item to drop based on min and max values
                int ammount = UnityEngine.Random.Range(lootTable.lootTable[randomID].minAmmountPerDrop, lootTable.lootTable[randomID].maxAmmountPerDrop);
                // Set item to be dropped
                Item item = lootTable.lootTable[randomID].item;

                // Call on the DropItem method with item and amount
                DropItem(item, ammount);
            }

            // Add kill score to players score
            target.GetComponent<PlayerStats>().ChangeScore(killScore);
            // Display a popup message with the kill score
            gameManagerScript.DisplayMessage(killScore.ToString(), gameObject, scoreAdditionPopupColour);

            // IF enemy is part of a spawner
            if (enemySpawner != null)
            {
                // Delete enemy from spawners spawned enemies list
                enemySpawner.GetComponent<SpawnerScript>().spawnedEnemies.Remove(gameObject);
            }

            // Destroy the enemies health bar
            Destroy(healthBar);
            // Destroy the enemy
            Destroy(gameObject);
            // Don’t continue (return)
            return;
        }

        // IF enemy has no health bar
        if (healthBar == null)
        {
            // Create new health bar using prefab
            GameObject newHealthBar = Instantiate(healthBarPrefab, canvas.transform.Find("LevelSpawns"));
            EnemyHealthBarScript newHealthBarScript = newHealthBar.GetComponent<EnemyHealthBarScript>();

            // Set health, max health, enemy object, and y offset values in the health bar script
            newHealthBarScript.maxHealth = maxHealth;
            newHealthBarScript.currentHealth = health;
            newHealthBarScript.enemyObject = gameObject.transform.Find("Enemy").gameObject;
            newHealthBarScript.yPosOffSet = HealthBarYPosOffset;

            // Set created health bar to created object
            healthBar = newHealthBar;
        }
        else
        {
            // Decrease the health bar value by the damage
            healthBar.GetComponent<EnemyHealthBarScript>().DecreaseEnemyHealthBar(damage);
        }
    }

    /// <summary>
    /// Handles dropping of items/loot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ammount"></param>
    private void DropItem(Item item, int ammount)
    {
        // make sure nothing is dropped if the ammount is 0
        if (ammount <= 0)
        {
            return;
        }

        // Spawn in a new dropped item
        GameObject newItemDrop = Instantiate(droppedItemPrefab);

        // Set an X & Y location based on object position, offset, and random scatter
        Vector2 enemyPosition = gameObject.transform.GetChild(0).position;
        float x = UnityEngine.Random.Range(enemyPosition.x + lootScatter, enemyPosition.x - lootScatter);
        float y = UnityEngine.Random.Range(enemyPosition.y + lootScatter, enemyPosition.y - lootScatter);
        newItemDrop.transform.position = new Vector2(x, y);

        // Assign the ItemPickup Script of the dropped item an item and amount
        newItemDrop.GetComponent<ItemPickup>().item = item;
        newItemDrop.GetComponent<ItemPickup>().ammount = ammount;
    }

    /// <summary>
    /// Handles melee attacks on target/player
    /// </summary>
    private void MeleeAttackPlayer()
    {
        // Directly call on the targets stats script’s SubtractHealth method with the damage
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
        // IF collision object is the target
        if (collision.gameObject == target)
        {
            // Set touching player to true
            touchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // IF collision object is the target
        if (collision.gameObject == target)
        {
            // Set touching player to false
            touchingPlayer = false;
        }
    }
}
