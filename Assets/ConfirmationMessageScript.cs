using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConfirmationMessageScript : MonoBehaviour
{
    /// <summary>
    /// When the confirm no button is pressed, this will cancel the action
    /// </summary>
    public void CancelConfirm()
    {
        // destroys the object
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// Called on when the player confirms for the main menu scene to be loaded
    /// </summary>
    public void ConfirmLoadMenu()
    {
        // destroy the object
        Destroy(gameObject, 0.1f);
        // load the main menu
        GameObject.Find("Level Loader Transition").GetComponent<LevelLoader>().LoadNewScene(1);
    }

    /// <summary>
    /// Called on when the player confirms to quit the game
    /// </summary>
    public void ConfirmedQuitGame()
    {
        // destroy the object
        Destroy(gameObject, 0.1f);
        // quit the game
        Application.Quit();
    }
}
