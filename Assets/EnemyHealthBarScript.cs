using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarScript : MonoBehaviour
{
    private GameManagerScript gameManagerScript = GameManagerScript.Instance;
    public float maxHealth;
    public float currentHealth;
    private float healthBarWidthIncrement;
    public GameObject enemyObject;
    public float yPosOffSet;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        float healthBarWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;

        healthBarWidthIncrement = healthBarWidth / maxHealth;

        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth * healthBarWidthIncrement);
    }

    // Update is called once per frame
    void Update()
    {
        // follow the enemy
        gameObject.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y + yPosOffSet, enemyObject.transform.position.z));
    }

    public void DecreaseEnemyHealthBar(float ammount)
    {
        currentHealth -= ammount;
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth * healthBarWidthIncrement);
    }
}
