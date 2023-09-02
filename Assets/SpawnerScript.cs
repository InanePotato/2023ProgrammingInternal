using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript;
    public GameObject spawnerIconPrefab;
    private GameObject currentIcon;

    [Header("Spawner Information")]
    public GameObject EnemyToSpawn;
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

        currentIcon = Instantiate(spawnerIconPrefab, gameManagerScript.mainCanvis.transform);
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
            
            // change the enemies range
            newEnemy.GetComponent<EnemyScript>().movementOffset = spawnRange;
            // assign this spawner to the enemy
            newEnemy.GetComponent<EnemyScript>().enemySpawner = gameObject;
            // move the enemy's 'home' location to the spawner
            newEnemy.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z - 0.01f);

            newEnemy.transform.parent = gameObject.transform;

            // move the enemy to a random location withen it's range
            newEnemy.transform.GetChild(0).position = new Vector2(gameObject.transform.position.x + Random.Range(-spawnRange.x, spawnRange.x), gameObject.transform.position.y + Random.Range(-spawnRange.y, spawnRange.y));
            // make sure the enemy is above the spawner
            newEnemy.transform.position = new Vector3(newEnemy.transform.position.x, newEnemy.transform.position.y, newEnemy.transform.position.z - 0.01f);

            // give the new enemy loot table info
            newEnemy.GetComponent<EnemyScript>().lootTable = spawnEnemyLootTable;
            newEnemy.GetComponent<EnemyScript>().minItemsFromLootTable = spawnEnemyMinItemsFromLootTable;
            newEnemy.GetComponent<EnemyScript>().maxItemsFromLootTable = spawnEnemyMaxItemsFromLootTable;

            // add new enemy to list of enemies
            spawnedEnemies.Add(newEnemy);
        }
    }
}
