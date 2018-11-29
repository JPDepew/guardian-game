using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public static PlayerStats instance;

    private int lives;
    private int score;
    public bool bigLaser = false;

    private void Awake()
    {
        instance = this;

        lives = 3;
    }

    public int GetLives()
    {
        return lives;
    }

    public void IncreaseScoreBy(int amount)
    {
        score += amount;
    }


    public int GetScore()
    {
        return score;
    }
    public void DecrementLives()
    {
        lives--;
    }

    public void IncrementLives()
    {
        lives++;
    }

    public void ResetAllPowerups()
    {
        bigLaser = false;
    }
}
