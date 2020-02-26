using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatform : MonoBehaviour, IDestroyable, IParentPlayer, IUnParentPlayer
{
    public PlatformType type;

    public delegate void FreeFallPlayerEvent();
    public event FreeFallPlayerEvent OnFreeFallPlayerHandler;

    public delegate void RecalculateDistanceEvent();
    public event RecalculateDistanceEvent OnRecalculateDistanceEvent;

    /// <summary>
    /// Logic for the Interaction with Base Platform(Grass).
    /// </summary>
    /// <param name="controller">Player who interacts with platform.</param>
    public virtual void Interact(Person controller)
    {
        if (controller.PlayerWon) return;
        //PrintObjectInteracting(controller, "Normal");
        controller.hasInteracatedWithSlide = false;

        CheckFallingFromHeight();

        if (controller.currentState != PersonState.STUNNED && controller.currentState != PersonState.DEAD) controller.currentState = PersonState.IDLE;
        controller.IsFreeFall = false;
    }

    /// <summary>
    /// Prints out the interaction when something touches the platform.
    /// </summary>
    /// <param name="controller"> Name of the player(Tim, Annie, AI, PVP Player).</param>
    /// <param name="platformName"> Name of the platform Player touched. </param>
    protected void PrintObjectInteracting(Person controller, string platformName)
    {
        Debug.Log($"Object: {controller.gameObject.name} is interacting with {platformName} platform!");
    }

    [SerializeField] protected Person _controller;

    [SerializeField] protected BoxCollider2D triggerCollider;
    [SerializeField] private Transform _playerPosition;

    #region UNITY FUNCTIONS
    public bool IsMovingLevel;
    public ShootDirection platformLocation;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector3 _direction;

    /*private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }*/
    protected virtual void Start()
    {
        if (IsMovingLevel)
        {
            switch (platformLocation)
            {
                case ShootDirection.DOWN:
                    _direction = Vector2.right;
                    break;
                case ShootDirection.LEFT:
                    _direction = Vector2.up;
                    break;
                case ShootDirection.UP:
                    _direction = Vector2.left;
                    break;
                case ShootDirection.RIGHT:
                    _direction = Vector2.down;
                    break;
            }
        }

        if (transform.parent.childCount > 2) _playerPosition = transform.parent.GetChild(2);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPositioned) return;
        if (collision.collider.CompareTag("Player")  || collision.collider.CompareTag("AI"))
        {
            //Debug.Log($"Player entered the platform: { gameObject.name } with position:{gameObject.transform.position}" );
            _controller = collision.collider.transform.parent.GetComponent<Person>();
            //ParentPlayer(_controller);
            StartCoroutine(_WaitForIdle(_controller));
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (!isPositioned) return;
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("AI"))
        {
            //Debug.Log($"Player left the platform: { gameObject.name }");
            if(OnFreeFallPlayerHandler != null) OnFreeFallPlayerHandler -= _controller.EnterInFreeFall;
            UnParentPlayer(_controller);
            _controller = null;

        }
    }

    public float distanceFromUpperPlatform;
    public float distanceFromUnderneathPlatfrom;

    protected virtual void OnEnable()
    {

        _controller = null;
        GetVerticalPlatformDistance();
        SubscribeSurroundingPlatforms();

        if (IsMovingLevel) LevelManager.LevelFinishedBuilding += GiveVelocity;
    }

    private void GiveVelocity()
    {
        _rigidbody.velocity = _direction * Globals.Instance.LevelMoveSpeed;
    }

    protected virtual void OnDisable()
    {   /*
        UnsubscribeSurroundingPlatforms();
        //If player is on platform and platform gets destroyed => change to true
        if (_controller != null)
        {
            _controller.EnterInFreeFall();//controller.IsFreeFall = true;
            if (OnFreeFallPlayerHandler != null) OnFreeFallPlayerHandler -= _controller.EnterInFreeFall;
            _controller = null;
        }*/

        if (IsMovingLevel) LevelManager.LevelFinishedBuilding -= GiveVelocity;

    }

    #endregion
    void SubscribeSurroundingPlatforms()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3 (0.2f, 0, 0), Vector2.right, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.left, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent += hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;
    }

    void UnsubscribeSurroundingPlatforms()
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.right, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

        hit = Physics2D.Raycast(transform.position + new Vector3(0.2f, 0, 0), Vector2.left, 20f, 1 << 8);
        if (hit) OnRecalculateDistanceEvent -= hit.collider.gameObject.GetComponent<BasePlatform>().GetVerticalPlatformDistance;

    }

    void GetVerticalPlatformDistance()
    {
        RaycastHit2D hit;

        //Up
        hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector2.up, 20f, 1 << 8);
        if (hit) distanceFromUpperPlatform = hit.distance;
        else distanceFromUpperPlatform = 20;

        //Down
        hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.2f, 0), Vector2.down, 20f, 1 << 8);
        if (hit) distanceFromUnderneathPlatfrom = hit.distance;
        else distanceFromUnderneathPlatfrom = 20;
    }
    
    protected void EnterFreeFall() => OnFreeFallPlayerHandler?.Invoke();

    public void SetUpTriggerCollider(BoxCollider2D col) { triggerCollider = col; }

    public void SetUpPlatformForBoss(ShootDirection platformLocation)
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerPosition = gameObject.transform.GetChild(0);
        this.platformLocation = platformLocation;
        IsMovingLevel = true;
    }

    private IEnumerator _WaitForIdle(Person controller)
    {
        yield return new WaitUntil( () => controller.currentState == PersonState.IDLE);
        OnFreeFallPlayerHandler += controller.EnterInFreeFall;
        ParentPlayer(controller);
        Interact(controller);
        //Debug.Log("Has entered the collision: " + controller.gameObject.name);
    }
    
    protected void CheckFallingFromHeight()
    {
        if (_controller == null) return;
        if (_controller.IsFreeFall && _controller.isDeadFromFall && !_controller.PlayerWon && !_controller.isReadyToDestroy) _controller.Death("Height");
    }

    #region Interface

    //If Player is on this platform and gets destroyed => enter free fall
    public virtual void DestroyObject()
    {
        if (_controller != null) _controller.Death("the explosion!");
        //OnRecalculateDistanceEvent?.Invoke();
        //OnFreeFallPlayerHandler?.Invoke();
    }

    public bool isPositioned = false;
    public void UnParentPlayer(Person controller)
    {
        if (_playerPosition != null && controller != null && isPositioned)
        {
            isPositioned = false;
            //Debug.Log("Tim has exited collider with the platform! " + type);
            controller.UnParentPerson();
        }
    }

    public void ParentPlayer(Person controller)
    {
        Debug.Log($"Parent: {controller}");
        if (_playerPosition != null && controller != null && !isPositioned)
        {
            isPositioned = true;
            controller.ParentPerson(_playerPosition.transform);

            controller.gameObject.transform.localPosition = Vector2.zero;
            //_playerPosition.transform.position;
            //new Vector3(_playerPosition.position.x, controller.gameObject.transform.position.y);
        }
    }
    #endregion



}