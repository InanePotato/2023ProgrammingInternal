using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarScript : MonoBehaviour
{
    // Declare class scope variables
    private GameManagerScript gameManagerScript = GameManagerScript.Instance;
    public float maxHealth;
    public float currentHealth;
    private float healthBarWidthIncrement;
    public GameObject enemyObject;
    public float yPosOffSet;

    // Start is called before the first frame update
    void Start()
    {
        // Get and set a reference to the game manager script
        gameManagerScript = GameManagerScript.Instance;

        // Get the width of the object
        float healthBarWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;

        // Calculate and set how much width to subtract for every 1 health
        healthBarWidthIncrement = healthBarWidth / maxHealth;

        // Set width of health bar value using current anchors
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth * healthBarWidthIncrement);
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure position is accurate using assigned enemies position and given offset
        gameObject.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(new Vector3(enemyObject.transform.position.x, enemyObject.transform.position.y + yPosOffSet, enemyObject.transform.position.z));
    }

    public void DecreaseEnemyHealthBar(float ammount)
    {
        // Subtract the amount from the current health
        currentHealth -= ammount;
        // Set new width of health bar value using current anchors
        gameObject.transform.GetChild(0).GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentHealth * healthBarWidthIncrement);
    }
}
