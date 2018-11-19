using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMaster : MonoBehaviour
{
    public Text highScoreText;
    public Text scoreText;

    float timer = 0;

    private void Start()
    {
        highScoreText.text = "High Score: " + Constants.S.highScore.ToString();
        scoreText.text = "Score: " + Data.Instance.score.ToString();
        Data.Instance.ResetAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(2);
        }
    }
}