using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats instance;

    private int lives;

    private void Awake()
    {
        instance = this;

        lives = 3;
    }

    public int GetLives()
    {
        return lives;
    }

    public void DecrementLives()
    {
        lives--;
    }

    public void IncrementLives()
    {
        lives++;
    }
}
