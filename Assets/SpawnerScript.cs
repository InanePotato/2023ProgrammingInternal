using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript;
    public GameObject spawnerIconPrefab;
    private GameObject currentIcon;

    [Header("Spawn Enemy")]
    public GameObject EnemyToSpawn;
    public float maxHealth;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float targetingRange;

    [Header("Spawner Information")]
    public bool canSpawn;
    public float spawnCooldown;
    private float spawnCooldownCount;
    public Vector2 spawnRange;
    public int maxSpawnEnemies;
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public LootTable spawnEnemyLootTable;
    public int spawnEnemyMinItemsFromLootTable;
    public int spawnEnemyMaxItemsFromLootTable;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        currentIcon = Instantiate(spawnerIconPrefab, gameManagerScript.mainCanvis.transform.Find("LevelSpawns"));
        var iconImage = currentIcon.GetComponent<UnityEngine.UI.Image>();
        iconImage.sprite = EnemyToSpawn.GetComponentInChildren<SpriteRenderer>().sprite;
        //iconImage.sprite = EnemyToSpawn.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;

        // move to appropriate place on screen
        currentIcon.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(gameObject.transform.position);

        spawnCooldownCount = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        currentIcon.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(gameObject.transform.position);

        if (canSpawn)
        {
            // IF no cooldown left
            if (spawnCooldownCount <= 0)
            {
                SpawnEnemy();
                spawnCooldownCount = spawnCooldown;
            }

            spawnCooldownCount -= Time.deltaTime;
        }
    }

    public void SpawnEnemy()
    {
        if (spawnedEnemies.Count < maxSpawnEnemies)
        {
            // spawn enemy in
            GameObject newEnemy = Instantiate(EnemyToSpawn);

            // get the new enemies script
            EnemyScript newEnemiesScript = newEnemy.GetComponent<EnemyScript>();

            // change the enemies range
            newEnemiesScript.movementOffset = spawnRange;
            // assign this spawner to the enemy
            newEnemiesScript.enemySpawner = gameObject;
            // move the enemy's 'home' location to the spawner
            newEnemy.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.01f);

            newEnemy.transform.parent = gameObject.transform;

            // move the enemy to a random location withen it's range
            newEnemy.transform.GetChild(0).position = new Vector2(gameObject.transform.position.x + Random.Range(-spawnRange.x, spawnRange.x), gameObject.transform.position.y + Random.Range(-spawnRange.y, spawnRange.y));
            // make sure the enemy is above the spawner
            newEnemy.transform.position = new Vector3(newEnemy.transform.position.x, newEnemy.transform.position.y, newEnemy.transform.position.z - 0.01f);

            // give the new enemy loot table info
            newEnemiesScript.lootTable = spawnEnemyLootTable;
            newEnemiesScript.minItemsFromLootTable = spawnEnemyMinItemsFromLootTable;
            newEnemiesScript.maxItemsFromLootTable = spawnEnemyMaxItemsFromLootTable;

            // changes enemies stats if required
            if (maxHealth > 0) { newEnemiesScript.maxHealth = maxHealth; }
            if (damage > 0) { newEnemiesScript.damage = damage; }
            if (attackRange > 0) { newEnemiesScript.attackRange = attackRange; }
            if (attackCooldown > 0) { newEnemiesScript.attackCooldown = attackCooldown; }
            if (targetingRange > 0) { newEnemiesScript.targetingRange = targetingRange; }

            // add new enemy to list of enemies
            spawnedEnemies.Add(newEnemy);
        }
    }
}
