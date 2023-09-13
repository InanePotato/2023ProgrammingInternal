using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuManagerScript : MonoBehaviour
{
    private void Start()
    {
        if (GameObject.Find("txtHighScores") != null)
        {
            string filePath = Application.streamingAssetsPath + "/HighScores.txt";
            if (System.IO.File.Exists(filePath))
            {
                FileInfo file = new FileInfo(filePath);
                StreamReader reader = file.OpenText();

                GlobalVariables.highscores.Clear();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    var values = line.Split(",");

                    GlobalVariables.Highscore newScore;
                    newScore.name = values[0];
                    newScore.score = int.Parse(values[1]);

                    GlobalVariables.highscores.Add(newScore);
                }

                GlobalVariables.highscores.Sort((x, y) => x.score - y.score);
                GlobalVariables.highscores.Reverse();

                GameObject.Find("txtHighScores").GetComponent<Text>().text = "HighScores:" + Environment.NewLine;
                for (int i = 0; i < 5; i++)
                {
                    if (GlobalVariables.highscores.Count - 1 >= i)
                    {
                        GameObject.Find("txtHighScores").GetComponent<Text>().text += (i + 1).ToString() + ". " + GlobalVariables.highscores[i].name + " - " + GlobalVariables.highscores[i].score + Environment.NewLine;
                    }
                    else
                    {
                        GameObject.Find("txtHighScores").GetComponent<Text>().text += (i + 1).ToString() + ". Not Set" + Environment.NewLine;
                    }
                }
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame(InputField input)
    {
        if (input.text != "" && !input.text.Contains(","))
        {
            GlobalVariables.playerName = input.text;
            GameObject.Find("Level Loader Transition").GetComponent<LevelLoader>().LoadNewScene(2);
        }
    }
}
