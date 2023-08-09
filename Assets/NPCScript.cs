using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    [Header("Defaults")]
    private GameManagerScript gameManagerScript;
    public InventoryManager playerInventory;
    public GameObject messagePrefab;

    [Header("Message")]
    public List<string> messages = new List<string>();
    private int messageIndex = 0;

    private GameObject currentMessageDisplay = null;

    public float messageShowTime;
    private float messageShowTimeCountdown = 0;

    public bool interacting = false;

    [Header("Trading")]
    public GameObject tradePanelPrefab;
    public GameObject tradeRowPrefab;

    public bool canTrade;
    private GameObject currentTradePanel;

    public List<Trade> trades = new List<Trade>();

    [Serializable]
    public struct Trade
    {
        public Item purchaseItem;
        public int purchaseItemAmmount;
        public Item costItem;
        public int costItemAmmount;
        public int tradeStock;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TerminateInteraction();
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
        else
        {
            // cooldown left, so...
            // decrease message show countdown time
            messageShowTimeCountdown -= Time.deltaTime;
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
        GameObject newMessageSpawn = Instantiate(messagePrefab, gameManagerScript.mainCanvis.transform);
        newMessageSpawn.transform.GetChild(0).gameObject.GetComponent<Text>().text = messages[index].ToString();

        currentMessageDisplay = newMessageSpawn;

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
        // If message display showing, destroy it
        if (currentMessageDisplay != null)
        {
            Destroy(currentMessageDisplay);
        }

        // show trading panel
        GameObject tradingPanel = Instantiate(tradePanelPrefab, gameManagerScript.mainCanvis.transform);
        currentTradePanel = tradingPanel;

        // fill trading panel with trades
        for (int i = 0; i < trades.Count; i++)
        {
            Trade trade = trades[i];

            GameObject newTradingRow = Instantiate(tradeRowPrefab, tradingPanel.transform);
            // show trade Item stuff
            newTradingRow.transform.Find("imgItem").gameObject.GetComponent<Image>().sprite = trade.purchaseItem.sprite;
            newTradingRow.transform.Find("txtItemAmmount").gameObject.GetComponent<Text>().text = trade.purchaseItemAmmount.ToString();
            // show trade cost stuff
            newTradingRow.transform.Find("imgCost").gameObject.GetComponent<Image>().sprite = trade.costItem.sprite;
            newTradingRow.transform.Find("txtCostAmmount").gameObject.GetComponent<Text>().text = trade.costItemAmmount.ToString();

            if (trade.tradeStock <= 0)
            {
                newTradingRow.GetComponent<Image>().color = new Color(255, 45, 5, 120);
            }
            else
            {
                // set trade button click event
                newTradingRow.transform.Find("btnTrade").gameObject.GetComponent<Button>().onClick.AddListener(() => PurchaseTrade(ref trade, newTradingRow));
            }
        }
    }

    public void PurchaseTrade(ref Trade trade, GameObject rowObject)
    {
        if (trade.tradeStock <= 0)
        {
            return;
        }

        // IF the cost item is coins, then act differently
        if (trade.costItem.type == Item.ItemType.currency)
        {
            // IF player can't afford purchase
            if (playerInventory.coins < trade.costItemAmmount)
            {
                Debug.Log("trade failed. Only have " + playerInventory.coins);
                return;
            }
        }
        else
        {
            // IF player can't afford purchase
            if (playerInventory.GetItemAmmount(trade.costItem) < trade.costItemAmmount)
            {
                Debug.Log("trade failed. Only have " + playerInventory.GetItemAmmount(trade.costItem));
                return;
            }
        }

        trade.tradeStock--;
        playerInventory.SubtractItemAmmount(trade.costItem, trade.costItemAmmount);
        playerInventory.AddItemAmmount(trade.purchaseItem, trade.purchaseItemAmmount);

        Debug.LogWarning("Purchase Item " + trade.purchaseItem.name + "x" + trade.purchaseItemAmmount.ToString() + Environment.NewLine +
                         "Purchase Cost " + trade.costItem.name + "x" + trade.costItemAmmount.ToString());

        if (trade.tradeStock <= 0)
        {
            //rowObject.GetComponent<Image>().color = new Color(180f, 25f, 0f, 80f);
            rowObject.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.15f);
        }
    }
}
