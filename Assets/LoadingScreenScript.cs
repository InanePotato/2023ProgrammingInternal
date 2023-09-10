using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenScript : MonoBehaviour
{
    public GameObject progressBar;
    public Animator animator;
    public LevelLoader levelLoader;
    public int nextSceneIndex;
    public float secondsPerMessage;
    [SerializeField]
    private List<string> messageOptions = new List<string>();
    public List<string> messagesToShow = new List<string>();
    public bool canChangeMessages = false;
    private float nextMessageCooldown;
    private int currentMessageID = 0;
    private string prefix = "Loading";
    private string suffix = ".";
    public float addDotCooldown;
    private float nextDotCooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        // set the initialtimer values
        nextDotCooldownTimer = addDotCooldown;
        nextMessageCooldown = secondsPerMessage;

        // set the length of the animation
        float animationtime = animator.GetCurrentAnimatorStateInfo(0).length;
        // set the path to the loading messages text file
        string messageFilePath = Application.streamingAssetsPath + "/LoadingMessages.txt";

        // IF the text file exists
        if (File.Exists(messageFilePath))
        {
            // read the whole text file and set a list to it's contents
            messageOptions = File.ReadAllLines(messageFilePath).ToList();

            // IF there are 0 or less lines in the text file
            if (messageOptions.Count() <= 0)
            {
                // don't continue / return
                return;
            }

            // set that messages can change
            canChangeMessages = true;

            // FOR the ammount of messages that will fit in the transition length (rounded up)
            for (int i = 0; i < Math.Ceiling(animationtime / secondsPerMessage); i++)
            {
                // get a random message ID
                int randomID = UnityEngine.Random.Range(0, messageOptions.Count() - 1);
                // add the message to the to show messages
                messagesToShow.Add(messageOptions[randomID]);
                // remove the message from the initial list (so no double ups)
                messageOptions.RemoveAt(randomID);
            }

            // set the initial prefix of the loading message
            prefix = messagesToShow[0];
            // display the loading message prefix and suffix on screen
            gameObject.GetComponent<Text>().text = prefix + suffix;

        }
    }

    // Update is called once per frame
    void Update()
    {
        // IF the message can change
        if (canChangeMessages)
        {
            // decrease cooldown timer appropriatly
            nextMessageCooldown -= Time.deltaTime;

            // IF no cooldown left
            if (nextMessageCooldown <= 0)
            {
                // IF still messages left to show
                if (currentMessageID != messagesToShow.Count() - 1)
                {
                    // reset cooldown timer
                    nextMessageCooldown = secondsPerMessage;
                    // get next message prefix to display
                    currentMessageID++;
                    prefix = messagesToShow[currentMessageID];
                    // update message display with new prefix
                    gameObject.GetComponent<Text>().text = prefix + suffix;
                }
                else
                {
                    // no messages left
                    // load the next scene
                    levelLoader.LoadNewScene(nextSceneIndex);
                }
            }
        }

        // decrease change suffix cooldown appropriatly
        nextDotCooldownTimer -= Time.deltaTime;
        // IF cooldown timer is up
        if (nextDotCooldownTimer <= 0)
        {
            // reset cooldown timer
            nextDotCooldownTimer = addDotCooldown;
            
            // IF suffix already contains 'max' dots
            if (suffix == ".....")
            {
                // set back to 1
                suffix = ".";
            }
            else
            {
                // otherwise just add 1 dot
                suffix += ".";
            }

            // update message display with new suffix
            gameObject.GetComponent<Text>().text = prefix + suffix;
        }
    }
}
