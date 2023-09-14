using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class StartMenuManagerScript : MonoBehaviour
{
    public GameObject confimationMessage;
    public GameObject canvas;

    private void Start()
    {
        // IF the scene contains the highscore text
        if (GameObject.Find("txtHighScores") != null)
        {
            // get file path
            string filePath = Application.streamingAssetsPath + "/HighScores.txt";
            // IF highscres file exists
            if (System.IO.File.Exists(filePath))
            {
                // set file location
                FileInfo file = new FileInfo(filePath);
                // open the file in a stream reader
                StreamReader reader = file.OpenText();

                // clear the highscores list
                GlobalVariables.highscores.Clear();

                // WHILE there are still more lines to read on the file
                while (!reader.EndOfStream)
                {
                    // read and split the line into values
                    string line = reader.ReadLine();
                    var values = line.Split(",");

                    // add values to highscores list
                    GlobalVariables.Highscore newScore;
                    newScore.name = values[0];
                    newScore.score = int.Parse(values[1]);

                    GlobalVariables.highscores.Add(newScore);
                }

                // sort the list numerically
                GlobalVariables.highscores.Sort((x, y) => x.score - y.score);
                GlobalVariables.highscores.Reverse();

                // set the start of the highscores display lise
                GameObject.Find("txtHighScores").GetComponent<Text>().text = "HighScores:" + Environment.NewLine;
                // for all 5 display highscores
                for (int i = 0; i < 5; i++)
                {
                    // IF this placing of highscore exists
                    if (GlobalVariables.highscores.Count - 1 >= i)
                    {
                        // add the value with placing
                        GameObject.Find("txtHighScores").GetComponent<Text>().text += (i + 1).ToString() + ". " + GlobalVariables.highscores[i].name + " - " + GlobalVariables.highscores[i].score + Environment.NewLine;
                    }
                    else
                    {
                        // otherwise, dosn't exist then display "not set"
                        GameObject.Find("txtHighScores").GetComponent<Text>().text += (i + 1).ToString() + ". Not Set" + Environment.NewLine;
                    }
                }
            }
        }
    }

    /// <summary>
    /// when the player presses the quit game button show confirm menu
    /// </summary>
    public void QuitGame()
    {
        // spawn in confirm menu
        Instantiate(confimationMessage, canvas.transform);
    }

    /// <summary>
    /// when player clicks play button
    /// </summary>
    /// <param name="input"></param>
    public void PlayGame(GameObject input)
    {
        // IF the payer name input is 'alowed'
        if (input.GetComponent<InputField>().text != "" && !input.GetComponent<InputField>().text.Contains(","))
        {
            // set the player name
            GlobalVariables.playerName = input.GetComponent<InputField>().text;
            // load the play menu scene
            GameObject.Find("Level Loader Transition").GetComponent<LevelLoader>().LoadNewScene(2);
        }
        else
        {
            // otherwsie, play error animation
            input.GetComponent<Animator>().SetTrigger("MessageError");
        }
    }
}
