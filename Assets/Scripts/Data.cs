using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Instance
    {
        get;
        set;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ResetAll()
    {
        score = 0;
        lives = 3;
    }

    public int score = 0;
    public int lives = 3;
    public int scoreCounter = 0;
}
