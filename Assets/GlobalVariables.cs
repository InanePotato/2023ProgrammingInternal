using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static string playerName;
    public static List<Highscore> highscores = new List<Highscore>();

    public struct Highscore
    {
        public string name;
        public int score;
    }
}
