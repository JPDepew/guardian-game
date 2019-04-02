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
    public GameObject watchAlien;
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
    public Text bonusText;
    public Text waveText;
    public Text instructionsText;
    public Text exit;

    private PlayerStats playerStats;
    private GameObject shipReference;
    private bool respawningCharacter;
    private Animator bonusTextAnimator;
    private AudioSource[] audioSources;

    private float waveCount = 0f;
    private bool firstSpawn = true;
    private int bonus;
    private int score;
    private int scoreTracker;
    private int alienDestroyedCountTracker;
    private int dstAsteroidsCanSpawnFromPlayer = 3;
    private float verticalHalfSize = 0;
    private bool currentWatchAlien;

    private Vector3 playerPosition;
    private Quaternion rotation;

    void Start()
    {
        gameState = GameState.STOPPED;
        playerStats = PlayerStats.instance;
        verticalHalfSize = Camera.main.orthographicSize;
        bonusTextAnimator = bonusText.GetComponent<Animator>();
        StartGame();
        Alien.onAlienDestroyed += OnAlienDestroyed;
        MutatedAlien.onMutatedAlienDestroyed += OnAlienDestroyed;
        Watch.onWatchDestroyed += OnWatchDestroyed;
        audioSources = GetComponents<AudioSource>();
        playerPosition = new Vector3(0, 0, 0);

        StartCoroutine(InstructionsTextFadeOut());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene(1);
        }
        HandleUI();
    }

    IEnumerator InstructionsTextFadeOut()
    {
        yield return new WaitForSeconds(3f);
        while (instructionsText.color.a > 0.05f)
        {
            instructionsText.color = new Color(instructionsText.color.r, instructionsText.color.g, instructionsText.color.b, instructionsText.color.a - 0.05f);
            exit.color = new Color(exit.color.r, exit.color.g, exit.color.b, exit.color.a - 0.05f);
            yield return null;
        }
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
            waveText.text = "Wave " + waveCount + " Complete";
            bonusText.text = "Surviving humans: " + (bonus / 500) + " x 500 = " + bonus + " bonus\nTotal Points: " + (playerStats.GetScore() + bonus);
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
                (int)Random.Range(side1.transform.position.x + 16, side2.transform.position.x - 16) :
                (int)Random.Range(side1.transform.position.x - 16, side2.transform.position.x + 16);
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
        if (waveCount % 6 == 0 && !currentWatchAlien)
        {
            currentWatchAlien = true;
            audioSources[0].Stop();
            Instantiate(watchAlien, new Vector2(shipReference.transform.position.x + 4, Camera.main.orthographicSize + 3), watchAlien.transform.rotation);
            yield return new WaitForSeconds(6);
            audioSources[1].Play();
        }
    }

    IEnumerator SpawnAlien(Vector2 alienPosition)
    {
        Transform tempTransform = Instantiate(alienSpawn, alienPosition, transform.rotation).transform;
        yield return new WaitForSeconds(alienSpawn.main.duration);
        Instantiate(alien, tempTransform.position, transform.rotation);
    }

    private void OnWatchDestroyed()
    {
        currentWatchAlien = false;
        audioSources[0].Play();
        audioSources[1].Stop();
    }

    private void OnAlienDestroyed()
    {
        alienDestroyedCountTracker++;
        if (alienDestroyedCountTracker == numberOfAliens - 2)
        {
            if (waveCount % 2 == 0)
            {
                Instantiate(flyingSaucer, new Vector2(shipReference.transform.position.x + 12, Camera.main.orthographicSize - 2), transform.rotation);
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

    public void RespawnPlayer()
    {
        playerPosition = shipReference.transform.position;
        rotation = shipReference.transform.rotation;
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
        shipReference = Instantiate(ship, new Vector2(playerPosition.x, 0), rotation);
    }

    // This is necessary
    // Without this, all is for naught.
    // There's some weird thing with reloading scenes, where an object disappears, but it is still subscribed to an event. What?
    // Who knows, but this solves the problem
    private void OnDestroy()
    {
        Alien.onAlienDestroyed -= OnAlienDestroyed;
        MutatedAlien.onMutatedAlienDestroyed -= OnAlienDestroyed;
        Watch.onWatchDestroyed -= OnWatchDestroyed;
    }

    IEnumerator NewScene()
    {
        yield return new WaitForSeconds(4);
        Data.Instance.score = playerStats.GetScore();
        SceneManager.LoadScene(3);
    }
}
