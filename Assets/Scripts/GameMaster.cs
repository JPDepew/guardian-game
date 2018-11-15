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

    public GameObject side1;
    public GameObject side2;

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
    private Animator bonusTextAnimator;

    private bool firstSpawn = true;
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
        bonusTextAnimator = bonusText.GetComponent<Animator>();
        StartGame();
        Alien.onAlienDestroyed += OnAlienDestroyed;
    }

    private void Update()
    {
        //Debug.Log(alienDestroyedCountTracker);
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
        livesText.text = "Lives: " + playerStats.GetLives().ToString();
        scoreText.text = playerStats.GetScore().ToString();
    }

    IEnumerator InstantiateNewWave()
    {
        if (!firstSpawn)
        {
            bonusText.gameObject.SetActive(true);
            bonusTextAnimator.Play("Wave End");
            bonusText.text = "Surviving humans: " + (bonus / 500) + " x 500 = " + bonus + " bonus";
        }
        else
        {
            bonusText.text = "";
        }
        firstSpawn = false;
        yield return new WaitForSeconds(bonusTextAnimator.GetCurrentAnimatorStateInfo(0).length);
        bonusText.text = "";
        bonusText.GetComponent<Animator>().StopPlayback();
        bonusText.gameObject.SetActive(false);
        PlayerStats.instance.IncreaseScoreBy(bonus);
        bonus = 0;
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
            int xRange = side1.transform.position.x > side2.transform.position.x ?
                (int)Random.Range(side1.transform.position.x + 35, side2.transform.position.x - 35) :
                (int)Random.Range(side1.transform.position.x - 35, side2.transform.position.x + 35);
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
            if (this != null)
            {
                StartCoroutine(InstantiateNewWave());
            }
        }
    }

    private void DealWithRemainingHumans()
    {
        Human[] humans = FindObjectsOfType<Human>();
        for (int i = 0; i < humans.Length; i++)
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
