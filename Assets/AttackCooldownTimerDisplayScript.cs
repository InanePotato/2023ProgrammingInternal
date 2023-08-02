using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class AttackCooldownTimerDisplayScript : MonoBehaviour
{
    public GameManagerScript gameManagerScript;
    public PlayerAttackScript attackScript;
    public GameObject player;
    public float yPosOffSet;
    public float attackCooldown;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameManagerScript.Instance;

        attackScript = player.GetComponent<PlayerAttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        var imageComponent = gameObject.GetComponent<Image>();

        if (imageComponent.fillAmount <= 0)
        {
            Destroy(gameObject);
        }

        float increment = 1/attackCooldown;
        float deduction = increment * attackScript.attackCooldownTime * Time.deltaTime;

        if (imageComponent.fillAmount - deduction < 0)
        {
            imageComponent.fillAmount = 0;
        }
        else
        {
            imageComponent.fillAmount -= deduction;
        }

        gameObject.GetComponent<RectTransform>().position = gameManagerScript.camera.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + yPosOffSet, player.transform.position.z));
    }
}
