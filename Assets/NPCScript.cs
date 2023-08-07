using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    [Header("Defaults")]
    private GameManagerScript gameManagerScript;

    public GameObject messagePrefab;

    [Header("Message")]
    public List<string> messages = new List<string>();
    private int messageIndex = 0;

    private GameObject currentMessageDisplay = null;
    
    public float messageShowTime;
    private float messageShowTimeCountdown = 0;

    public bool interacting = false;

    private void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // IF not interacting don't do anything
        if (!interacting)
        {
            return;
        }


        // IF no cooldown left
        if (interacting && messageShowTimeCountdown <= 0)
        {
            messageIndex++;

            // IF still messages to show
            if (messageIndex <= messages.Count)
            {
                ChangeMessage(messageIndex - 1);
            }
            else
            {
                // not interaction messages left to show so...
                // stop interaction
                TerminateInteraction();
            }
        }
        else
        {
            // cooldown left, so...
            // decrease message show countdown time
            messageShowTimeCountdown -= Time.deltaTime;
        }
    }

    public void StartInteraction()
    {
        // reset any message index priviously selected
        messageIndex = 0;
        // set interacting to true
        interacting = true;
        // set cooldown to nothing
        messageShowTimeCountdown = 0;
    }

    private void ChangeMessage(int index)
    {
        // destroy privious message if there is one
        if (currentMessageDisplay != null)
        {
            Destroy(currentMessageDisplay);
        }

        // spawn new message display at correct location
        //GameObject newMessageSpawn = Instantiate(messagePrefab, gameManagerScript.mainCanvis.transform);
        //newMessageSpawn.transform.position = gameManagerScript.camera.WorldToScreenPoint(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z));
        //newMessageSpawn.GetComponent<Text>().text = messages[index].ToString();

        //currentMessageDisplay = newMessageSpawn;

        Debug.Log(messages[index].ToString());

        //reset show time
        messageShowTimeCountdown = messageShowTime;
    }

    public void TerminateInteraction()
    {
        // set interacting to false
        interacting = false;

        // If message display showing, destroy it
        if (currentMessageDisplay != null)
        {
            Destroy(currentMessageDisplay);
        }
    }
}
