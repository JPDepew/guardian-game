using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject alien;
    public GameObject ship;
    public GameObject human;
    public Text scoreText;
    public Text livesText;
    public float numberOfAliens;
    public float playerRespawnDelay = 1f;
    public float instantiateNewWaveDelay = 2f;

    public enum GameState { RUNNING, STOPPED }
    public GameState gameState;

    private PlayerStats playerStats;
    private ShipController shipController;
    private GameObject shipReference;
    private bool respawningCharacter;
    private bool instantiatingNewWave;
    private float playerRespawnTimer = 1f;
    private float instantiateNewWaveTimer = 2f;

    private int score;
    private int scoreTracker;
    private int asteroidCountTracker;
    private int dstAsteroidsCanSpawnFromPlayer = 3;
    private float verticalHalfSize = 0;
    private float horizontalHalfSize = 0;

    void Start()
    {
        gameState = GameState.STOPPED;
        playerStats = PlayerStats.instance;
        verticalHalfSize = Camera.main.orthographicSize;
        horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;
        Asteroid.onSmallAsteroidDestroyed += OnSmallAsteroidDestroyed;
    }

    private void Update()
    {
        if (scoreTracker > 10000)
        {
            scoreTracker = 0;
            playerStats.IncrementLives();
        }

        HandleUI();
        HandleRespawnTimer();
        HandleWaveTimer();
    }

    private void StartGame()
    {
        gameState = GameState.RUNNING;
        asteroidCountTracker = 0;
        shipReference = Instantiate(ship);
        shipController = shipReference.GetComponent<ShipController>();

        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        for (int i = 0; i < asteroids.Length; i++)
        {
            Destroy(asteroids[i].gameObject);
        }

        InstantiateNewWave();
    }

    private void HandleUI()
    {
        scoreText.text = score.ToString();
        //livesText.text = playerStats.GetLives().ToString() + "x";
    }

    private void HandleRespawnTimer()
    {
        if (respawningCharacter)
        {
            if (Time.time > playerRespawnTimer)
            {
                shipReference = Instantiate(ship);
                shipController = shipReference.GetComponent<ShipController>();
                respawningCharacter = false;
            }
        }
    }

    private void HandleWaveTimer()
    {
        if (instantiatingNewWave)
        {
            if (Time.time > instantiateNewWaveTimer)
            {
                InstantiateNewWave();
                instantiatingNewWave = false;
            }
        }
    }

    private void InstantiateNewWave()
    {
        for (int i = 0; i < numberOfAliens; i++)
        {
            int xRange = (int)Random.Range(-horizontalHalfSize, horizontalHalfSize);
            int yRange = (int)Random.Range(-verticalHalfSize, verticalHalfSize);

            Vector2 asteroidPositon = new Vector2(xRange, yRange);
            if ((asteroidPositon - (Vector2)shipReference.transform.position).magnitude < dstAsteroidsCanSpawnFromPlayer)
            {
                i--; // This is probably really sketchy, I know... But it works really well...
            }
            else
            {
                Instantiate(alien, asteroidPositon, transform.rotation);
            }
        }
    }

    private void OnSmallAsteroidDestroyed()
    {
        asteroidCountTracker++;
        if (asteroidCountTracker >= numberOfAliens * 4)
        {
            numberOfAliens++;
            asteroidCountTracker = 0;
            instantiateNewWaveTimer = Time.time + instantiateNewWaveDelay;
            instantiatingNewWave = true;
        }
    }

    private void EndGame()
    {
        gameState = GameState.STOPPED;
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreTracker += amount;
    }

    public void RespawnPlayer()
    {
        if (playerStats.GetLives() > 0)
        {
            respawningCharacter = true;
            playerRespawnTimer = Time.time + playerRespawnDelay;
        }
        else
        {
            StartCoroutine(RestartSceneTimer());
        }
    }

    IEnumerator RestartSceneTimer()
    {
        float targetTime = Time.time + 3f;
        while (Time.time < targetTime)
        {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
