using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConfirmationMessageScript : MonoBehaviour
{
    public void CancelConfirm()
    {
        Destroy(gameObject, 0.1f);
    }

    public void ConfirmLoadMenu()
    {
        Destroy(gameObject, 0.1f);
        GameObject.Find("Level Loader Transition").GetComponent<LevelLoader>().LoadNewScene(1);
    }

    public void ConfirmedQuitGame()
    {
        Destroy(gameObject, 0.1f);
        Application.Quit();
    }
}
