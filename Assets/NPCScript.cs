using System;
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
    }

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
            else if (canTrade && currentTradePanel != null)
            {
                // show trade window
                ShowTradingWindow();
            }
            else if (currentTradePanel == null)
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

            GameObject newTradingRow = Instantiate(tradeRowPrefab, gameManagerScript.mainCanvis.transform);
            // show trade Item stuff
            newTradingRow.transform.Find("imgItem").gameObject.GetComponent<Image>().sprite = trade.purchaseItem.sprite;
            newTradingRow.transform.Find("txtItemAmmount").gameObject.GetComponent<Text>().text = trade.purchaseItemAmmount.ToString();
            // show trade cost stuff
            newTradingRow.transform.Find("imgCost").gameObject.GetComponent<Image>().sprite = trade.costItem.sprite;
            newTradingRow.transform.Find("txtCostAmmount").gameObject.GetComponent<Text>().text = trade.costItemAmmount.ToString();
            // set trade button click event
            newTradingRow.transform.Find("btnTrade").gameObject.GetComponent<Button>().onClick.AddListener(() => PurchaseTrade(trade));
        }
    }

    public void PurchaseTrade(Trade trade)
    {

        Debug.LogWarning("Purchase Item " + trade.purchaseItem.name + "x" + trade.purchaseItemAmmount.ToString() + Environment.NewLine +
                         "Purchase Cost " + trade.costItem.name + "x" + trade.costItemAmmount.ToString());
    }
}
