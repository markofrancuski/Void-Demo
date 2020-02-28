using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    #region EVENTS
    public delegate void BossTakeDamageEventHandler(int maxhHealth, int damage);
    public static event BossTakeDamageEventHandler BossTookDamage;

    public delegate void BossChangedDirectionEventHandler();
    public static event BossChangedDirectionEventHandler BossChangedDirection;
    #endregion

    #region BOSS STATS
    [SerializeField] private bool _alreadyTakenDamage;
    private bool waitForDamage;
    public int Health;
    [SerializeField] private int _health;

    public float moveSpeed;
    public ShootDirection direction;

    [SerializeField] private float fireRate;
    [SerializeField] private float tempFireRate;
    [SerializeField] private GameObject[] spawnPoints;

    #endregion

    [SerializeField] private Vector2 _size;

    [SerializeField] private GameObject projectile;
    [SerializeField] private Quaternion angle;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    #region UNITY FUNCTIONS
    private void Start()
    {
        tempFireRate = fireRate;
        _health = Health;
        _size = transform.localScale;
    }

    private void Update()
    {
        if(isShooting)
        {
            tempFireRate -= Time.deltaTime;
            if (tempFireRate <= 0) Fire();
        }

        //float xPos = Mathf.Lerp(-0.56f, 0.56f, Time.deltaTime);
        //gameObject.transform.position = new Vector3(xPos, 2.22f, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            TakeDamage();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collision.isTrigger)
        {
            Invoke("WaitForOneTenth", 0.1f);
        }
    }
    void WaitForOneTenth()
    {
        _alreadyTakenDamage = false;
    }
    #endregion

    #region HELPER FUNCTIONS
    //Private functions
    /// <summary>
    /// Calculates the rotation for projectile according to the direction. 
    /// </summary>
    private void RotateProjectile()
    {
        switch (direction)
        {
            case ShootDirection.UP:
                angle = Quaternion.Euler(new Vector3(0, 0, 90));
                break;
            case ShootDirection.DOWN:
                angle = Quaternion.Euler(new Vector3(0, 0, -90));
                break;
            case ShootDirection.RIGHT:
                angle = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case ShootDirection.LEFT:
                angle = Quaternion.Euler(new Vector3(0, 0, -180));
                break;
        }
    }
    /// <summary>
    /// Gets the direction in which projectile should move according to the direction.
    /// </summary>
    /// <returns></returns>
    Vector2 GetMoveDirection()
    {
        switch (direction)
        {
            case ShootDirection.UP: return Vector2.up;
            case ShootDirection.DOWN: return Vector2.down;
            case ShootDirection.RIGHT: return Vector2.right;
            case ShootDirection.LEFT: return Vector2.left;
            default: return Vector2.right;

        }
    }
    private bool isShooting;
    /// <summary>
    /// Gets called when Boss takes damage.
    /// </summary>
    private void TakeDamage()
    {
        if (_alreadyTakenDamage) return;
        _alreadyTakenDamage = true;
        _health--;
        BossTookDamage?.Invoke(Health, _health);
        if (_health <= 0 && !waitForDamage)
        {
            waitForDamage = true;
            Debug.Log("Boss is dead! spawn next boss");
            LevelManager.Instance.CheckBossFinished();
            //EnableBoss();
        }
        else TintRed();

    }
    /// <summary>
    /// Changes the boss scale based on the where boss is located.
    /// </summary>
    /// <param name="horizontal">Flip horizontal or vertial</param>
    private void ResizeBoss(bool horizontal)
    {
        if (horizontal) transform.localScale = new Vector2(_size.x, _size.y);
        else transform.localScale = new Vector2(_size.y, _size.x);

    }
    //Public Functions
    /// <summary>
    /// Spawns the projectile and gives him direction where to move.
    /// </summary>
    public void Fire()
    {
        int rnd = Random.Range(0, 2);
        GameObject GO = Instantiate(projectile);
        GO.transform.position = spawnPoints[rnd].transform.position;
        GO.transform.rotation = angle;

        GO.GetComponent<Projectile>().SetUpProjectile(3, GetMoveDirection());
        tempFireRate = fireRate;
    }
    /// <summary>
    /// Checks if boss is shooting.
    /// </summary>
    /// <returns></returns>
    public bool GetActive() { return isShooting; }
    /// <summary>
    /// Switches on/off value for shooting.
    /// </summary>
    public void TurnActive() => isShooting = !isShooting;
    /// <summary>
    /// Gets called after the boss die, to reset values and re-position the boss.
    /// </summary>
    public void EnableBoss()
    {
        direction = (ShootDirection)LevelManager.Instance.bossLevelIndex;
        BossChangedDirection?.Invoke();

        _health = Health;
        //BossTookDamage?.Invoke(Health, _health);
        UpdateBossHP();
        //Resize the sprite
        //Rotate and move projectile spawners
        switch (direction)
        {
            case ShootDirection.UP:
                ResizeBoss(true);
                transform.position = new Vector3(0, -Globals.Instance.movePaceVertical, 0);
                break;
            case ShootDirection.DOWN:
                ResizeBoss(true);
                transform.position = new Vector3(0, Globals.Instance.movePaceVertical, 0);
                break;
            case ShootDirection.RIGHT:
                ResizeBoss(false);
                transform.position = new Vector3(-Globals.Instance.movePaceHorizontal, 0, 0);
                break;
            case ShootDirection.LEFT:
                ResizeBoss(false);
                transform.position = new Vector3(Globals.Instance.movePaceHorizontal, 0, 0);
                break;
            default:
                break;
        }
        waitForDamage = false;
    }

    private void TintRed()
    {
        _spriteRenderer.color = Color.red;
        Invoke("TintNormal", 0.1f);
    }
    private void TintNormal()
    {
        _spriteRenderer.color = Color.white;
    }

    private void UpdateBossHP()
    {
        LevelManager.Instance.UpdateBossHealth(Health, _health);
    }
    #endregion



}
