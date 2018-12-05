using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public GameObject alien;
    public GameObject flyingSaucer;
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
    private GameObject shipReference;
    private bool respawningCharacter;
    private Animator bonusTextAnimator;

    private float waveCount = 0f;
    private bool firstSpawn = true;
    private int bonus;
    private int score;
    private int scoreTracker;
    private int alienDestroyedCountTracker;
    private int dstAsteroidsCanSpawnFromPlayer = 3;
    private float verticalHalfSize = 0;

    void Start()
    {
        gameState = GameState.STOPPED;
        playerStats = PlayerStats.instance;
        verticalHalfSize = Camera.main.orthographicSize;
        bonusTextAnimator = bonusText.GetComponent<Animator>();
        StartGame();
        Alien.onAlienDestroyed += OnAlienDestroyed;
        MutatedAlien.onMutatedAlienDestroyed += OnAlienDestroyed;
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
        Data.Instance.score = 0;
        gameState = GameState.RUNNING;
        alienDestroyedCountTracker = 0;
        shipReference = Instantiate(ship);

        StartCoroutine(InstantiateNewWave());
    }

    private void HandleUI()
    {
        livesText.text = "Lives: " + playerStats.GetLives().ToString();
        scoreText.text = playerStats.GetScore().ToString();
    }

    IEnumerator InstantiateNewWave()
    {
        if (waveCount > 0)
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
        waveCount++;
    }

    private IEnumerator InstantiateHumans()
    {
        for (int i = 0; i < 10; i++)
        {
            int xRange = side1.transform.position.x > side2.transform.position.x ?
                (int)Random.Range(side1.transform.position.x + 18, side2.transform.position.x - 18) :
                (int)Random.Range(side1.transform.position.x - 18, side2.transform.position.x + 18);
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
                (int)Random.Range(side1.transform.position.x + 18, side2.transform.position.x - 18) :
                (int)Random.Range(side1.transform.position.x - 18, side2.transform.position.x + 18);
            int yRange = (int)Random.Range(-verticalHalfSize, verticalHalfSize);

            Vector2 alienPositon = new Vector2(xRange, yRange);
            while (shipReference == null)
            {
                yield return new WaitForSeconds(0.2f);
            }
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
        Transform tempTransform = Instantiate(alienSpawn, alienPosition, transform.rotation).transform;
        yield return new WaitForSeconds(alienSpawn.main.duration);
        Instantiate(alien, tempTransform.position, transform.rotation);
    }

    private void OnAlienDestroyed()
    {
        Debug.Log("Alien Destoryed");
        alienDestroyedCountTracker++;
        if(alienDestroyedCountTracker == numberOfAliens - 2)
        {
            if (waveCount % 2 == 0)
            {
                Instantiate(flyingSaucer, new Vector2(0, Camera.main.orthographicSize + 20), transform.rotation);
            }
        }
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
            if (!(humans[i].curState == Human.State.DEAD))
            {
                bonus += 500;
            }
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
        playerStats.ResetAllPowerups();
        if (playerStats.GetLives() > 0)
        {
            StartCoroutine(RespawnPlayerTimer());
        }
        else
        {
            StartCoroutine(NewScene());
        }
    }

    IEnumerator RespawnPlayerTimer()
    {
        yield return new WaitForSeconds(playerRespawnDelay);
        shipReference = Instantiate(ship);
    }

    private void OnDestroy()
    {
        Alien.onAlienDestroyed -= OnAlienDestroyed;
        MutatedAlien.onMutatedAlienDestroyed -= OnAlienDestroyed;
    }

    IEnumerator NewScene()
    {
        yield return new WaitForSeconds(4);
        Data.Instance.score = playerStats.GetScore();
        SceneManager.LoadScene(3);
    }
}
