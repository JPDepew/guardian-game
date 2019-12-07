using UnityEngine;
using UnityEngine.SceneManagement;

public class Constants : MonoBehaviour
{
    public int highScore = 0;
    public int score = 0;
    public float wrapDst = 40;

    string playerPrefHighScoreKey = "playerHighScore";

    static public Constants S;

    private void Awake()
    {
        S = this;

        SetPlayerPrefs();
    }

    public void SetHighScore()
    {
        highScore = score;
        PlayerPrefs.SetInt(playerPrefHighScoreKey, highScore);
    }

    void SetPlayerPrefs()
    {
        if (PlayerPrefs.HasKey(playerPrefHighScoreKey))
        {
            highScore = PlayerPrefs.GetInt(playerPrefHighScoreKey);
        }
        else
        {
            PlayerPrefs.SetInt(playerPrefHighScoreKey, highScore);
        }
    }

    public void resetScore()
    {
        score = 0;
    }

    public void setScore(int newScore)
    {
        score = newScore;
    }
}
