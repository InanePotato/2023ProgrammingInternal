using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class AttackCooldownTimerDisplayScript : MonoBehaviour
{
    // Declare class scope variables
    public GameManagerScript gameManagerScript;
    public PlayerAttackScript attackScript;
    public GameObject player;
    public float yPosOffSet;
    public float attackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        // Get and set game manager script reference
        gameManagerScript = GameManagerScript.Instance;

        // Get and set player attack script reference
        attackScript = player.GetComponent<PlayerAttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the image component of the attached object
        var imageComponent = gameObject.GetComponent<Image>();

        // IF timer display is finished
        if (imageComponent.fillAmount <= 0)
        {
            // Destroy this object
            Destroy(gameObject);
        }

        // Calculate change in cooldown to change fill amount by
        float increment = 1/attackCooldown;
        float deduction = increment * attackScript.attackCooldownTime * Time.deltaTime;

        // IF Change leads to less than 0
        if (imageComponent.fillAmount - deduction < 0)
        {
            // Set object fill amount to 0
            imageComponent.fillAmount = 0;
        }
        else
        {
            // Take off calculates fill amount
            imageComponent.fillAmount -= deduction;
        }

        // Ensure object is displayed appropriately based on player location and offset
        gameObject.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + yPosOffSet, player.transform.position.z));
    }
}
