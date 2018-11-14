using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject alien;
    public GameObject ship;
    public GameObject human;
    public Text bonusText;
    public ParticleSystem alienSpawn;

    public float numberOfAliens;
    public float playerRespawnDelay = 10f;
    public float instantiateNewWaveDelay = 2f;

    public enum GameState { RUNNING, STOPPED }
    public GameState gameState;

    public Text scoreText;
    public Text livesText;

    private PlayerStats playerStats;
    private ShipController shipController;
    private GameObject shipReference;
    private bool respawningCharacter;

    private int bonus;
    private int score;
    private int scoreTracker;
    private int alienDestroyedCountTracker;
    private int dstAsteroidsCanSpawnFromPlayer = 3;
    private float verticalHalfSize = 0;
    private float horizontalHalfSize = 0;

    void Start()
    {
        gameState = GameState.STOPPED;
        playerStats = PlayerStats.instance;
        verticalHalfSize = Camera.main.orthographicSize;
        horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;
        StartGame();
        Alien.onAlienDestroyed += OnAlienDestroyed;
    }

    private void Update()
    {
        if (scoreTracker > 10000)
        {
            scoreTracker = 0;
            playerStats.IncrementLives();
        }

        HandleUI();
    }

    private void StartGame()
    {
        gameState = GameState.RUNNING;
        alienDestroyedCountTracker = 0;
        shipReference = Instantiate(ship);
        shipController = shipReference.GetComponent<ShipController>();

        StartCoroutine(InstantiateNewWave());
    }

    private void HandleUI()
    {
        livesText.text = playerStats.GetLives().ToString() + "x";
        scoreText.text = playerStats.GetScore().ToString();
    }

    IEnumerator InstantiateNewWave()
    {
        // Add small screen showing bonus for humans
        //bonusText.text = "Surviving humans: " + (bonus / 500) + " Bonus: " + bonus;
        yield return new WaitForSeconds(instantiateNewWaveDelay);
        PlayerStats.instance.IncreaseScoreBy(bonus);
        StartCoroutine(InstantiateAliens());
        StartCoroutine(InstantiateHumans());
    }

    private IEnumerator InstantiateHumans()
    {
        for (int i = 0; i < 20; i++)
        {
            int xRange = (int)Random.Range(shipReference.transform.position.x - 60, shipReference.transform.position.x + 60);
            float yRange = -4.3f;

            Vector2 humanPositon = new Vector2(xRange, yRange);

            Instantiate(human, humanPositon, transform.rotation);

            yield return null;
        }
    }

    private IEnumerator InstantiateAliens()
    {
        for (int i = 0; i < numberOfAliens; i++)
        {
            int xRange = (int)Random.Range(shipReference.transform.position.x - 50, shipReference.transform.position.x + 50);
            int yRange = (int)Random.Range(-verticalHalfSize, verticalHalfSize);

            Vector2 alienPositon = new Vector2(xRange, yRange);
            if ((alienPositon - (Vector2)shipReference.transform.position).magnitude < dstAsteroidsCanSpawnFromPlayer)
            {
                i--; // This is probably really sketchy, I know... But it works really well...
            }
            else
            {
                StartCoroutine("SpawnAlien", alienPositon);
            }
            yield return null;
        }
    }

    IEnumerator SpawnAlien(Vector2 alienPosition)
    {
        Instantiate(alienSpawn, alienPosition, transform.rotation);
        yield return new WaitForSeconds(alienSpawn.main.duration);
        Instantiate(alien, alienPosition, transform.rotation);
    }

    private void OnAlienDestroyed()
    {
        alienDestroyedCountTracker++;
        if (alienDestroyedCountTracker >= numberOfAliens)
        {
            numberOfAliens++;
            alienDestroyedCountTracker = 0;
            DealWithRemainingHumans();
            StartCoroutine(InstantiateNewWave());
        }
    }

    private void DealWithRemainingHumans()
    {
        Human[] humans = FindObjectsOfType<Human>();
        for(int i = 0; i < humans.Length; i++)
        {
            Destroy(humans[i].gameObject);
            bonus += 500;
        }
    }

    private void EndGame()
    {
        gameState = GameState.STOPPED;
    }

    public void IncreaseScore(int amount)
    {
        PlayerStats.instance.IncreaseScoreBy(amount);
        scoreTracker += amount;
    }

    public void RespawnPlayer()
    {
        if (playerStats.GetLives() > 0)
        {
            StartCoroutine(RespawnPlayerTimer());
        }
        else
        {
            StartCoroutine(RestartSceneTimer());
        }
    }

    IEnumerator RespawnPlayerTimer()
    {
        yield return new WaitForSeconds(playerRespawnDelay);
        shipReference = Instantiate(ship);
        shipController = shipReference.GetComponent<ShipController>();
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
