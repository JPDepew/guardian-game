using System.Collections;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public GameObject gunPosition;
    public GameObject bullet;
    public GameObject bulletDisinfect;
    public GameObject explosion;
    public ParticleSystem fuelParticleSystem;
    public GameObject leftShip;

    public Transform particleSystemPosLeft;
    public Transform particleSystemPosRight;

    public float acceleration = 0.1f;
    public float maxSpeed = 2;

    public float linearInterpolationTime = 0.2f;
    public float destroyWaitTime = 10;

    public GameObject bigLaser;
    public Human human { get; set; }
    private AudioSource[] audioSources;
    private Vector2 direction;
    private SpriteRenderer spriteRenderer;
    private PlayerStats playerStats;
    public bool shouldDestroyShip { get; set; }
    private bool canShoot = true;

    private float invulnerabilityTime = 2f;
    private float invulnerabilityTargetTime;
    private BoxCollider2D boxCollider;
    private bool shouldBeInvulnerable = true;

    float verticalHalfSize;

    private void Start()
    {
        playerStats = PlayerStats.instance;
        audioSources = GetComponents<AudioSource>();
        verticalHalfSize = Camera.main.orthographicSize;
        invulnerabilityTargetTime = Time.time + invulnerabilityTime;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;
    }

    void Update()
    {
        if (!shouldDestroyShip)
        {
            GetInput();
        }
        else
        {
            if (audioSources[1].isPlaying)
            {
                audioSources[1].Stop();
            }
        }
        HandleInvulnerability();
        transform.position = transform.position + (Vector3)direction * Time.deltaTime;
    }

    private void GetInput()
    {
        // Side to side movement
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            leftShip.SetActive(true);
            spriteRenderer.enabled = false;
            if (direction.x > -maxSpeed)
            {
                direction += acceleration * Vector2.left;
            }
            fuelParticleSystem.transform.rotation = Quaternion.Euler(new Vector3(180, -90, 0));
            fuelParticleSystem.transform.position = particleSystemPosRight.position;
            if (!fuelParticleSystem.isEmitting)
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
            leftShip.SetActive(false);
            spriteRenderer.enabled = true;
            if (direction.x < maxSpeed)
            {
                direction += acceleration * Vector2.right;
            }
            fuelParticleSystem.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            fuelParticleSystem.transform.position = particleSystemPosLeft.position;
            if (!fuelParticleSystem.isEmitting)
            {
                fuelParticleSystem.Play();
            }
            if (!audioSources[1].isPlaying)
            {
                audioSources[1].Play();
            }
        }
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.Space))
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
        if (Input.GetKey(KeyCode.Space))
        {
            if (leftShip.activeSelf)
            {
                if (direction.x < maxSpeed)
                {
                    direction += acceleration * Vector2.right;
                }
                fuelParticleSystem.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                fuelParticleSystem.transform.position = particleSystemPosRight.position;
                if (!fuelParticleSystem.isEmitting)
                {
                    fuelParticleSystem.Play();
                }
                if (!audioSources[1].isPlaying)
                {
                    audioSources[1].Play();
                }
            }
            else
            {
                if (direction.x > -maxSpeed)
                {
                    direction += acceleration * Vector2.left;
                }
                fuelParticleSystem.transform.rotation = Quaternion.Euler(new Vector3(180, -90, 0));
                fuelParticleSystem.transform.position = particleSystemPosLeft.position;
                if (!fuelParticleSystem.isEmitting)
                {
                    fuelParticleSystem.Play();
                }
                if (!audioSources[1].isPlaying)
                {
                    audioSources[1].Play();
                }
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
        if (Input.GetKeyDown(KeyCode.Z) && !shouldDestroyShip && canShoot)
        {
            if (!playerStats.bigLaser)
            {
                audioSources[0].Play();
                GameObject tempBullet = Instantiate(bullet, gunPosition.transform.position, transform.rotation);

                tempBullet.transform.localScale = leftShip.activeSelf == true ?
                    new Vector2(-tempBullet.transform.localScale.x, tempBullet.transform.localScale.y) :
                    new Vector2(tempBullet.transform.localScale.x, tempBullet.transform.localScale.y);

                canShoot = false;
                StartCoroutine(WaitBetweenShooting(false));
            }
            else
            {
                audioSources[2].Play();

                StartCoroutine(ShootBigLaser());
                canShoot = false;
                StartCoroutine(WaitBetweenShooting(false));
            }
        }
        if (Input.GetKeyDown(KeyCode.X) && !shouldDestroyShip && canShoot)
        {
            audioSources[0].Play();
            GameObject tempBullet = Instantiate(bulletDisinfect, gunPosition.transform.position, transform.rotation);

            tempBullet.transform.localScale = leftShip.activeSelf == true ?
                new Vector2(-tempBullet.transform.localScale.x, tempBullet.transform.localScale.y) :
                new Vector2(tempBullet.transform.localScale.x, tempBullet.transform.localScale.y);

            canShoot = false;
            StartCoroutine(WaitBetweenShooting(true));
        }
    }

    IEnumerator ShootBigLaser()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject tempBullet = Instantiate(bigLaser, gunPosition.transform.position, transform.rotation);
        tempBullet.transform.localScale = leftShip.activeSelf == true ?
            new Vector2(-tempBullet.transform.localScale.x, tempBullet.transform.localScale.y) :
            new Vector2(tempBullet.transform.localScale.x, tempBullet.transform.localScale.y);
    }

    IEnumerator WaitBetweenShooting(bool disinfect)
    {
        float waitTime = playerStats.bigLaser && !disinfect ? 0.45f : 0.1f;
        yield return new WaitForSeconds(waitTime);
        canShoot = true;
    }

    private void HandleInvulnerability()
    {
        if (shouldBeInvulnerable)
        {
            if (Time.time > invulnerabilityTargetTime)
            {
                boxCollider.enabled = true;
                shouldBeInvulnerable = false;
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            }
        }
    }

    /// <summary>
    /// Destroys the player, instantiates the explosion particle system, which has the explosion sound on it, and decrements lives.
    /// </summary>
    void DestroySelf()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        playerStats.DecrementLives();
        playerStats.ResetAllPowerups();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Alien")
        {
            collision.GetComponent<Enemy>().DamageSelf(100, transform.position);
            DestroySelf();
            FindObjectOfType<GameMaster>().RespawnPlayer();
        }
        if (collision.tag == "AlienBullet")
        {
            Destroy(collision.gameObject);
            FindObjectOfType<GameMaster>().RespawnPlayer();
            DestroySelf();
        }
        if (collision.tag == "Human")
        {
            if (!human)
            {
                human = collision.transform.GetComponent<Human>();
                if (human.curState == Human.State.FALLING)
                {
                    human.transform.SetParent(transform);
                    human.transform.position = new Vector2(transform.position.x, transform.position.y - 0.5f);
                    human.curState = Human.State.RESCUED;
                }
                else
                {
                    human = null;
                }
            }
        }
    }
}
