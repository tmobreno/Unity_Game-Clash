using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredData : MonoBehaviour
{
    public int[] highScores = new int[3] { 50000, 20000, 10000 };

    public int[] loopNumber = new int[3] { 2, 1, 0 };

    public int[] levelNumber = new int[3] { 0, 0, 5 };

    public static StoredData instance;

    private void Awake()
    {
        instance = this;

        highScores[0] = PlayerPrefs.GetInt("First");
        highScores[1] = PlayerPrefs.GetInt("Second");
        highScores[2] = PlayerPrefs.GetInt("Third");

        loopNumber[0] = PlayerPrefs.GetInt("FirstLoop");
        loopNumber[1] = PlayerPrefs.GetInt("SecondLoop");
        loopNumber[2] = PlayerPrefs.GetInt("ThirdLoop");

        levelNumber[0] = PlayerPrefs.GetInt("FirstLevel");
        levelNumber[1] = PlayerPrefs.GetInt("SecondLevel");
        levelNumber[2] = PlayerPrefs.GetInt("ThirdLevel");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ResetScores();
        }
    }

    public void UpdateHighScores(int newScore, int newLoop, int newLevel)
    {
        DetermineScorePlacement(newScore, newLoop, newLevel);
        PlayerPrefs.SetInt(("First"), highScores[0]);
        PlayerPrefs.SetInt(("Second"), highScores[1]);
        PlayerPrefs.SetInt(("Third"), highScores[2]);


        PlayerPrefs.SetInt(("FirstLoop"), loopNumber[0]);
        PlayerPrefs.SetInt(("SecondLoop"), loopNumber[1]);
        PlayerPrefs.SetInt(("ThirdLoop"), loopNumber[2]);

        PlayerPrefs.SetInt(("FirstLevel"), levelNumber[0]);
        PlayerPrefs.SetInt(("SecondLevel"), levelNumber[1]);
        PlayerPrefs.SetInt(("ThirdLevel"), levelNumber[2]);
    }

    private void DetermineScorePlacement(int newScore, int newLoop, int newLevel)
    {
        if (newScore > highScores[0])
        {
            highScores[2] = highScores[1];
            highScores[1] = highScores[0];
            highScores[0] = newScore;

            loopNumber[2] = loopNumber[1];
            loopNumber[1] = loopNumber[0];
            loopNumber[0] = newLoop;

            levelNumber[2] = levelNumber[1];
            levelNumber[1] = levelNumber[0];
            levelNumber[0] = newLevel;
            return;
        }
        if(newScore > highScores[1])
        {
            highScores[2] = highScores[1];
            highScores[1] = newScore;

            loopNumber[2] = loopNumber[1];
            loopNumber[1] = newLoop;

            levelNumber[2] = levelNumber[1];
            levelNumber[1] = newLevel;
            return;
        }
        if (newScore > highScores[2])
        {
            highScores[2] = newScore;

            loopNumber[2] = newLoop;

            levelNumber[2] = newLevel;
            return;
        }
        return;
    }

    private void ResetScores()
    {
        PlayerPrefs.SetInt(("First"), 50000);
        PlayerPrefs.SetInt(("Second"), 20000);
        PlayerPrefs.SetInt(("Third"), 10000);

        PlayerPrefs.SetInt(("FirstLoop"), 2);
        PlayerPrefs.SetInt(("SecondLoop"), 1);
        PlayerPrefs.SetInt(("ThirdLoop"), 0);

        PlayerPrefs.SetInt(("FirstLevel"), 0);
        PlayerPrefs.SetInt(("SecondLevel"), 0);
        PlayerPrefs.SetInt(("ThirdLevel"), 5);
    }
}
