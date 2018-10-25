using UnityEngine;

public class ShipController : MonoBehaviour
{
    public GameObject gunPosition;
    public GameObject bullet;
    public GameObject explosion;
    public ParticleSystem fuelParticleSystem;

    public float acceleration = 0.1f;
    public float speed = 1;
    public float linearInterpolationTime = 0.2f;
    public float maxLookSpeed = 5;

    private AudioSource[] audioSources;
    private Vector2 direction;
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats;
    private bool shouldDestroyShip;
    private float targetTime;

    private float invulnerabilityTime = 2f;
    private float invulnerabilityTargetTime;
    private PolygonCollider2D polyCollider2D;
    private bool shouldBeInvulnerable = true;

    float verticalHalfSize;
    float horizontalHalfSize;

    private void Start()
    {
        playerStats = PlayerStats.instance;
        audioSources = GetComponents<AudioSource>();
        verticalHalfSize = Camera.main.orthographicSize;
        horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;
        invulnerabilityTargetTime = Time.time + invulnerabilityTime;
        fuelParticleSystem = GetComponent<ParticleSystem>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

        polyCollider2D = GetComponent<PolygonCollider2D>();
        polyCollider2D.enabled = false;
    }

    void Update()
    {
        GetInput();
        //HandleWrapping();
        HandleDestroyingShip();
        HandleInvulnerability();
        transform.position = transform.position + (Vector3)direction * Time.deltaTime;
    }

    private void GetInput()
    {
        // Side to side movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            direction += acceleration * Vector2.left;
            if (!fuelParticleSystem.isPlaying)
            {
                fuelParticleSystem.Play();
            }
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].Play();
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            direction += acceleration * Vector2.right;
            if (!fuelParticleSystem.isPlaying)
            {
                fuelParticleSystem.Play();
            }
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].Play();
            }
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if (Mathf.Abs(direction.x) > 0.01f)
            {
                direction = Vector2.Lerp(direction, Vector2.zero, linearInterpolationTime);
                if (fuelParticleSystem.isPlaying)
                {
                    fuelParticleSystem.Stop();
                }
                if (audioSources[1].isPlaying)
                {
                    audioSources[1].Stop();
                }
            }
            else
            {
                direction = new Vector2(0, direction.y);
            }
        }

        // Up and down movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            direction += acceleration * Vector2.up;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            direction += acceleration * Vector2.down;
        }
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.DownArrow))
        {
            if (Mathf.Abs(direction.y) > 0.01f)
            {
                direction = Vector2.Lerp(direction, Vector2.zero, linearInterpolationTime);
            }
            else
            {
                direction = new Vector2(direction.x, 0);
            }
        }

        // Checking to make sure it is not off the screen
        if (transform.position.y <= -verticalHalfSize + 1 && direction.y < 0)
        {
            direction = new Vector2(direction.x, 0);
        }
        if (transform.position.y >= verticalHalfSize - 0.5f && direction.y > 0)
        {
            direction = new Vector2(direction.x, 0);
        }

        // Shooting
        if (Input.GetKeyDown(KeyCode.Z) && !shouldDestroyShip)
        {
            audioSources[0].Play();
            GameObject tempBullet = Instantiate(bullet, gunPosition.transform.position, transform.rotation);
            tempBullet.transform.localScale = Mathf.Sign(transform.localScale.x) == -1 ? new Vector2(-tempBullet.transform.localScale.x, tempBullet.transform.localScale.y) : new Vector2(tempBullet.transform.localScale.x, tempBullet.transform.localScale.y);
        }
    }

    private void HandleDestroyingShip()
    {
        if (shouldDestroyShip)
        {
            if (Time.time > targetTime)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// If the ship goes off the screen, it is wrapped around to the other side
    /// </summary>
    //private void HandleWrapping()
    //{
    //    verticalHalfSize = Camera.main.orthographicSize;
    //    horizontalHalfSize = verticalHalfSize * Screen.width / Screen.height;

    //    if (transform.position.x > horizontalHalfSize)
    //    {
    //        transform.position = new Vector2(-horizontalHalfSize, transform.position.y);
    //    }
    //    if (transform.position.x < -horizontalHalfSize)
    //    {
    //        transform.position = new Vector2(horizontalHalfSize, transform.position.y);
    //    }
    //}

    private void HandleInvulnerability()
    {
        if (shouldBeInvulnerable)
        {
            if (Time.time > invulnerabilityTargetTime)
            {
                polyCollider2D.enabled = true;
                shouldBeInvulnerable = false;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Alien")
        {
            shouldDestroyShip = true;
            targetTime = Time.time + 1f;
            audioSources[1].Play();
            GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            GetComponent<Collider2D>().enabled = false;
            playerStats.DecrementLives();

            collision.GetComponent<Asteroid>().DestroyAsteroid();

            Instantiate(explosion, transform.position, transform.rotation);
            FindObjectOfType<GameMaster>().RespawnPlayer();
        }
    }
}
