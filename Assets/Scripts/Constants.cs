using UnityEngine;
using UnityEngine.SceneManagement;

public class Constants : MonoBehaviour
{
    public int highScore = 0;

    string playerPrefHighScoreKey = "playerHighScore";

    static public Constants S;

    private void Awake()
    {
        S = this;

        SetPlayerPrefs();
    }

    public void SetHighScore(int newHighScore)
    {
        highScore = newHighScore;
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
}
