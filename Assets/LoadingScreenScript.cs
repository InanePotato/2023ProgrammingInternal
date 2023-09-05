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
        nextDotCooldownTimer = addDotCooldown;
        nextMessageCooldown = secondsPerMessage;

        float animationtime = animator.GetCurrentAnimatorStateInfo(0).length;
        string messageFilePath = Application.streamingAssetsPath + "/LoadingMessages.txt";

        if (File.Exists(messageFilePath))
        {
            messageOptions = File.ReadAllLines(messageFilePath).ToList();

            if (messageOptions.Count() <= 0)
            {
                return;
            }

            canChangeMessages = true;

            for (int i = 0; i < Math.Ceiling(animationtime / secondsPerMessage); i++)
            {
                int randomID = UnityEngine.Random.Range(0, messageOptions.Count() - 1);
                messagesToShow.Add(messageOptions[randomID]);
                messageOptions.RemoveAt(randomID);
            }

            var imageComponent = progressBar.GetComponent<Image>();
            prefix = messagesToShow[0];
            gameObject.GetComponent<Text>().text = prefix + suffix;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canChangeMessages)
        {
            nextMessageCooldown -= Time.deltaTime;

            if (nextMessageCooldown <= 0)
            {
                if (currentMessageID != messagesToShow.Count() - 1)
                {
                    nextMessageCooldown = secondsPerMessage;
                    currentMessageID++;
                    var imageComponent = progressBar.GetComponent<Image>();
                    prefix = messagesToShow[currentMessageID];
                    gameObject.GetComponent<Text>().text = prefix + suffix;
                }
                else
                {
                    levelLoader.LoadNewScene(nextSceneIndex);
                }
            }
        }

        nextDotCooldownTimer -= Time.deltaTime;
        if (nextDotCooldownTimer <= 0)
        {
            nextDotCooldownTimer = addDotCooldown;
            
            if (suffix == ".....")
            {
                suffix = ".";
            }
            else
            {
                suffix += ".";
            }

            gameObject.GetComponent<Text>().text = prefix + suffix;
        }
    }
}
