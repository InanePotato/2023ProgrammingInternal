using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    [Header("Defaults")]
    private GameManagerScript gameManagerScript;
    public InventoryManager playerInventory;
    public GameObject messagePrefab;
    public KeyCode skipKey = KeyCode.Tab;

    [Header("Message")]
    public List<string> messages = new List<string>();
    private int messageIndex = 0;

    private GameObject currentMessageDisplay = null;

    public float timePerWord;
    private float timePerWordCount = 0;
    private List<string> words = new List<string>();

    public bool interacting = false;

    public bool firstInteractionComplete = false;

    [Header("Trading")]
    public GameObject tradePanelPrefab;
    public GameObject tradeRowPrefab;

    public bool canTrade;
    private GameObject currentTradePanel;

    public List<Trade> trades = new List<Trade>();

    [Serializable]
    public class Trade
    {
        public Item purchaseItem;
        public int purchaseItemAmmount;
        public Item costItem;
        public int costItemAmmount;
        public int tradeStock;
        public bool mustComplete;
    }

    private void Start()
    {
        gameManagerScript = GameManagerScript.Instance;
        playerInventory = InventoryManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // IF not interacting don't do anything
        if (!interacting)
        {
            return;
        }

        // if escape pressed, stop interaction
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    TerminateInteraction();
        //    return;
        //}

        // IF is interacting
        if (interacting)
        {
            // IF skip key pressed
            if (Input.GetKeyDown(skipKey) && currentMessageDisplay != null)
            {
                if (messageIndex >= messages.Count)
                {
                    CompleteInteractionMessages();
                }
                else
                {
                    messageIndex++;
                    ChangeMessage(messageIndex);
                }

                return;
            }

            // IF can do next word and still more words
            if (timePerWordCount <= 0 && words.Count > 0 && currentMessageDisplay != null)
            {
                NextWord();
            }

            // IF messages complete
            if (messageIndex > messages.Count)
            {
                CompleteInteractionMessages();
            }

            timePerWordCount -= Time.deltaTime;
        }
    }

    private void CompleteInteractionMessages()
    {
        // IF this is first interaction
        if (firstInteractionComplete == false)
        {
            TerminateInteraction();
            firstInteractionComplete = true;
        }
        else if (canTrade && currentTradePanel == null)
        {
            // show trade window
            ShowTradingWindow();
        }
        else if (!canTrade)
        {
            // not interaction messages left to show & no trading so...
            // stop interaction
            TerminateInteraction();
        }
    }

    public void StartInteraction()
    {
        // pause game
        gameManagerScript.gamePaused = true;
        // reset any message index priviously selected
        messageIndex = 0;
        // set interacting to true
        interacting = true;
        // set cooldown to nothing
        timePerWordCount = 0;

        //start showing messages
        ChangeMessage(messageIndex);
    }

    private void ChangeMessage(int index)
    {
        if (index > messages.Count - 1)
        {
            return;
        }

        // destroy privious message if there is one
        if (currentMessageDisplay != null)
        {
            Destroy(currentMessageDisplay);
        }

        // spawn new message display at correct location
        GameObject newMessageSpawn = Instantiate(messagePrefab, gameManagerScript.mainCanvis.transform.Find("NPCSpawns"));
        newMessageSpawn.transform.GetChild(0).gameObject.GetComponent<Text>().text = "";
        currentMessageDisplay = newMessageSpawn;

        words.Clear();

        string[] splitMessage = messages[index].Split(" ");
        foreach (string word in splitMessage)
        {
            words.Add(word);
        }

        Debug.Log(messages[index].ToString());

        //reset show time
        timePerWordCount = timePerWord;
    }

    private void NextWord()
    {
        timePerWordCount = timePerWord;

        if (words.Count <= 0)
        {
            return;
        }

        currentMessageDisplay.transform.GetChild(0).gameObject.GetComponent<Text>().text += words[0] + " ";
        words.RemoveAt(0);
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

        // IF trading window showing, destroy it
        if (currentTradePanel != null)
        {
            Destroy(currentTradePanel);
        }

        // un-pause game
        gameManagerScript.gamePaused = false;
    }

    private void ShowTradingWindow()
    {
        bool showOne = false;
        if (trades[0].mustComplete == true && trades[0].tradeStock > 0)
        {
            showOne = true;
        }

        // If message display showing, destroy it
        if (currentMessageDisplay != null)
        {
            Destroy(currentMessageDisplay);
        }

        // show trading panel
        GameObject tradingPanel = Instantiate(tradePanelPrefab, gameManagerScript.mainCanvis.transform.Find("NPCSpawns"));
        currentTradePanel = tradingPanel;

        // fill trading panel with trades
        for (int i = 0; i < trades.Count; i++)
        {
            Trade trade = trades[i];
            Debug.Log(trade.costItem.itemName);

            GameObject newTradingRow = Instantiate(tradeRowPrefab, tradingPanel.transform);
            // show trade Item stuff
            newTradingRow.transform.Find("imgItem").gameObject.GetComponent<Image>().sprite = trade.purchaseItem.sprite;
            newTradingRow.transform.Find("txtItemAmmount").gameObject.GetComponent<Text>().text = trade.purchaseItemAmmount.ToString();
            // show trade cost stuff
            newTradingRow.transform.Find("imgCost").gameObject.GetComponent<Image>().sprite = trade.costItem.sprite;
            newTradingRow.transform.Find("txtCostAmmount").gameObject.GetComponent<Text>().text = trade.costItemAmmount.ToString();

            if (trade.tradeStock <= 0 )
            {
                newTradingRow.GetComponent<Image>().color = new Color(255, 45, 5, 120);
            }
            else
            {
                // set trade button click event
                newTradingRow.transform.Find("btnTrade").gameObject.GetComponent<Button>().onClick.AddListener(() => PurchaseTrade(i, newTradingRow));
            }

            // if only one shold show, this will run after the first one has been added nd stop it
            if (showOne)
            {
                break;
            }
        }
    }

    public void PurchaseTrade(int tradeID, GameObject rowObject)
    {
        if (trades[tradeID].tradeStock <= 0)
        {
            return;
        }

        // IF the cost item is coins, then act differently
        if (trades[tradeID].costItem.type == Item.ItemType.currency)
        {
            // IF player can't afford purchase
            if (playerInventory.coins < trades[tradeID].costItemAmmount)
            {
                gameManagerScript.DisplayMessage("Insificient Funds", gameObject, Color.red);
                Debug.Log("trade failed. Only have " + playerInventory.coins);
                return;
            }
        }
        else
        {
            // IF player can't afford purchase
            if (playerInventory.GetItemAmmount(trades[tradeID].costItem) < trades[tradeID].costItemAmmount)
            {
                gameManagerScript.DisplayMessage("Insificient Funds", gameObject, Color.red);
                Debug.Log("trade failed. Only have " + playerInventory.GetItemAmmount(trades[tradeID].costItem));
                return;
            }
        }

        trades[tradeID].tradeStock--;
        playerInventory.SubtractItemAmmount(trades[tradeID].costItem, trades[tradeID].costItemAmmount);
        playerInventory.AddItemAmmount(trades[tradeID].purchaseItem, trades[tradeID].purchaseItemAmmount);

        Debug.LogWarning("Purchase Item " + trades[tradeID].purchaseItem.name + "x" + trades[tradeID].purchaseItemAmmount.ToString() + Environment.NewLine +
                         "Purchase Cost " + trades[tradeID].costItem.name + "x" + trades[tradeID].costItemAmmount.ToString());

        if (trades[tradeID].tradeStock <= 0)
        {
            //rowObject.GetComponent<Image>().color = new Color(180f, 25f, 0f, 80f);
            rowObject.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.15f);
        }
    }
}
