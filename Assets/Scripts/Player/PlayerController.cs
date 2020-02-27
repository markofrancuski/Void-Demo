using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using MEC;
using UnityEngine.Events;
using Pixelplacement.TweenSystem;

public class PlayerController : Person, IDestroyable
{

    #region EVENT/DELEGATE DECLARATION

    public delegate void OnBoundarySet(float x, float y);
    public static event OnBoundarySet onBoundarySet;

    public delegate void PlayerMovedEventHandler();
    public static event PlayerMovedEventHandler PlayerFinishedMoving;

    public event UnityAction OnSlimeStuck;


    //public delegate void OnPlayerDie();
    //public static event OnPlayerDie OnPlayerDieEvent;

    #endregion

    #region MOVEMENT VARIABLES
    //public LinkedList<string> movementList;

    public delegate void OnMovedEventHandler();
    public static OnMovedEventHandler OnMoved;
    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        currentState = PersonState.IDLE;
        if (LevelManager.Instance.IsBossLevel) LevelManager.LevelFinishedBuilding += SubscribeBossMovement;
        else
        {
            StartCoroutine("_MoveNormalPlayerCoroutine");
            nextPosition = transform.position;
        }

        if (personIndex == 0)
        {
            //Subscribe to the movement
        }
        isReadyToDestroy = false;

    }

    private void Update()
    {
#if UNITY_EDITOR
        Debug.DrawLine(transform.position, transform.position - new Vector3(0, transform.localScale.y / 2 + +0.1f, 0), Color.red);
#endif
    }

    private void OnEnable()
    {
        LevelManager.Instance.SubscribePlayer(personIndex, this);
        currentState = PersonState.IDLE;
        //SlidePlatform.OnSlidePlatformInteractEvent += AddFirstMove;

        LevelManager.ResetLevel += () => Destroy(gameObject);
        LevelManager.GivePlayerWin += PlayerWonLevel;
        LevelManager.BossKilled += PlayerResetPositionAfterKillingBoss;

        InputManager.OnSwipedEvent += AddMove;
        InputManager.OnPlayerStateCheckEvent += GetPlayerState;

        InputManager.OnUnControlPlayer += ClearList;

        InteractableManager.OnShieldActiveCheckEvent += () => IsProtected;
        InteractableManager.OnShieldActivateEvent += ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent += () => IsWeaponBoostActive.currentValue;
        InteractableManager.GetPlayerTeamEvent += () => Team;

        GameManager.CheckPlayerLifeSaverEvent += GetLifeSaver;
        GameManager.ReviveButtonClickedEvent += ChangePlayerExtraLife;
        GameManager.ReviveButtonClickedEvent += OnReviveClick;

        //Test
        GameManager.ResetSceneEvent += () => { IsExtraLifeActive.currentValue = true; };
        //OnPlayerDieEvent += LevelManager.Instance.ShowDeathPanel;
    }

    private void OnDisable()
    {

        //SlidePlatform.OnSlidePlatformInteractEvent -= AddFirstMove;

        LevelManager.ResetLevel -= () => Destroy(gameObject);
        LevelManager.GivePlayerWin -= PlayerWonLevel;
        LevelManager.BossKilled -= PlayerResetPositionAfterKillingBoss;

        InputManager.OnSwipedEvent -= AddMove;
        InputManager.OnPlayerStateCheckEvent -= GetPlayerState;
        InputManager.OnUnControlPlayer -= ClearList;

        InteractableManager.OnShieldActiveCheckEvent -= () => IsProtected;
        InteractableManager.OnShieldActivateEvent -= ActivateShield;
        InteractableManager.OnWeaponBoostActivateCheckEvent -= () => IsWeaponBoostActive.currentValue;
        InteractableManager.GetPlayerTeamEvent -= () => Team;

        GameManager.CheckPlayerLifeSaverEvent -= GetLifeSaver;
        GameManager.ReviveButtonClickedEvent -= ChangePlayerExtraLife;
        GameManager.ReviveButtonClickedEvent -= OnReviveClick;
        //Test
        GameManager.ResetSceneEvent -= () => { IsExtraLifeActive.currentValue = true; };
        //OnPlayerDieEvent -= LevelManager.Instance.ShowDeathPanel;
    }

    private void OnBecameInvisible()
    {
        if (PlayerWon) return;
        if (Globals.Instance.isSceneReady && !isReadyToDestroy) Death("Invisible");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            Death("Falling of the world");
        }
    }

    #endregion

    #region CHARACTER INTERACTION VARIABLES

    [Header("CHARACTER INTERACTION VARIABLES")]
    [SerializeField] private GameObject shieldBarrierGO;

    [SerializeField] private BoolValue IsWeaponBoostActive;
    [SerializeField] private BoolValue IsExtraLifeActive;

    [SerializeField] private bool isProtected;
    public bool IsProtected {
        get { return isProtected; }
        set { isProtected = value; }
    }

    //private int waitForTurns = 3;
    #endregion

    #region COROUTINES

    //private IEnumerator moveFunction;

    private IEnumerator _MoveNormalPlayerCoroutine()
    {
        while (true)
        {

            if (movementList.Count != 0 && currentState == PersonState.IDLE && !IsFreeFall && !hasInteracatedWithSlide)
            {
                //if (InputManager.Instance.isControllable)
                //{

                Vector3 _pos = GetMovement(movementList[0]);

                if (ValidateBoundary(_pos) )
                {
                    nextPosition = _pos;
                    MovePlayer();
                    Timing.RunCoroutine(_WaitForHalfOfTweenCoroutine().CancelWith(gameObject));
                    //Wait Tween duration
                    yield return new WaitForSeconds(Globals.Instance.tweenDuration);
                }
                else
                {
                    Timing.RunCoroutine(_WaitForHalfOfTweenCoroutine().CancelWith(gameObject));
                    yield return new WaitForSeconds(Globals.Instance.tweenDuration); // or wait one frame 
                    HandleTweenFinished();
                }

                //}
                ////Cancel the chain movement
                //else movementList.Clear();

            }
            yield return new WaitUntil(() => currentState == PersonState.IDLE);
        }
    }

    private IEnumerator _MoveBossPlayerCoroutine()
    {
        movementList = new List<string>();
        LevelManager.LevelFinishedBuilding -= SubscribeBossMovement;
        while (true)
        {
            if(movementList.Count != 0 && currentState == PersonState.IDLE && !IsFreeFall && !hasInteracatedWithSlide && !isAtEdge && !isFrozen)
            {
                Vector3 _pos = GetMovement(movementList[0]);

                if (CheckEdgeAndNextMove(_pos, LevelManager.Instance.bossScript.direction))
                {
                    lastPosition = transform.position;
                    isAtEdge = true;
                }

                if (ValidateBoundary(_pos))
                {
                    nextPosition = _pos;
                    MovePlayer();
                    Timing.RunCoroutine(_WaitForHalfOfTweenCoroutine().CancelWith(gameObject));
                    //Wait Tween duration
                    yield return new WaitForSeconds(Globals.Instance.tweenDuration);
                }
                else
                {
                    Timing.RunCoroutine(_WaitForHalfOfTweenCoroutine().CancelWith(gameObject));
                    yield return new WaitForSeconds(Globals.Instance.tweenDuration); // or wait one frame 
                    HandleTweenFinished();
                }
            }
            yield return new WaitUntil(() => currentState == PersonState.IDLE);
        }
    }

    private IEnumerator _ActivateShieldCoroutine()
    {
        isProtected = true;
        shieldBarrierGO.SetActive(true);
        yield return new WaitForSeconds(3);
        isProtected = false;
        shieldBarrierGO.SetActive(false);
    }

    private IEnumerator _InteractableCoroutine(float time, UnityAction interactMethod)
    {
        currentState = PersonState.INTERACTING;
        // InteractEvent += interactMethod;
        // InteractEvent?.Invoke();
        yield return new WaitForSeconds(time);
        currentState = PersonState.IDLE;
        // InteractEvent -= interactMethod;
    }

    private IEnumerator<float> _WaitForHalfOfTweenCoroutine()
    {
        yield return Timing.WaitForSeconds(Globals.Instance.tweenDuration / 3);
        if (personIndex == 0) OnMoved?.Invoke();
    }

    private delegate void WaitForTimeDelegate();
    /// <summary>
    /// Wait for full tween time duration and executes provided function after.
    /// </summary>
    /// <param name="customFunction">Function to be executed after tween duration.</param>
    /// <returns></returns>
    private IEnumerator<float> _WaitForFullTweenCoroutine(WaitForTimeDelegate customFunction)
    {
        yield return Timing.WaitForSeconds(Globals.Instance.tweenDuration);
        customFunction();
    }
    /// <summary>
    /// Wait for provided time duration and executes provided function after.
    /// </summary>
    /// <param name="time">Time to wait in seconds.</param>
    /// <param name="customFunction">Function to be executed after time.</param>
    /// <returns></returns>
    private IEnumerator<float> _WaitForTimeCoroutine(float time, WaitForTimeDelegate customFunction)
    {
        yield return Timing.WaitForSeconds(time);
        customFunction();
    }
    
    #endregion

    #region EVENT/DELEGATE FUNCTIONS

    public void ChangePlayerExtraLife()
    {
        IsExtraLifeActive.currentValue = !IsExtraLifeActive.currentValue;
    }

    public bool GetLifeSaver() => IsExtraLifeActive.currentValue;

    public PersonState GetPlayerState()
    {
        return currentState;
    }

    private void PlayerWonLevel() => PlayerWon = true;

    private void PlayerResetPositionAfterKillingBoss()
    {
        _moveBossTween.Stop();

        ClearList();
        //isReadyToDestroy = true;
        RigidbodyConstraints2D constraints = GetComponent<Rigidbody2D>().constraints;
        constraints = RigidbodyConstraints2D.FreezePosition;

        gameObject.transform.SetParent(null);
        isAtEdge = false;
        gameObject.transform.position = nextPosition = Vector3.zero;
        currentState = PersonState.IDLE;
        constraints = RigidbodyConstraints2D.None;
        //Destroy(gameObject);
        /*isAtEdge = false;
        Timing.RunCoroutine(_WaitForTimeCoroutine(0.5f, () => 
                {
                    transform.localPosition = Vector3.zero;
                    LevelManager.Instance.SceneReady();
                } ).CancelWith(this.gameObject));*/

    }

    #endregion

    #region HELPER FUNCTIONS
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    [SerializeField]private bool isAtEdge;
    private Vector2 lastPosition;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nextPos"> Next Position of player.</param>
    /// <param name="dir"> Where boss  is spawned, which side.</param>
    /// <returns></returns>
    private bool CheckEdgeAndNextMove(Vector2 nextPos, ShootDirection dir)
    {
        switch (dir)
        {
            case ShootDirection.DOWN:
                if (nextPos.y >= Globals.Instance.movePaceVertical && transform.position.y <= 0.15f) return true;
                break;
            case ShootDirection.LEFT:
                if (nextPos.x >= Globals.Instance.movePaceHorizontal && transform.position.x == 0) return true;
                break;
            case ShootDirection.UP:
                if (nextPos.y <= -Globals.Instance.movePaceVertical && transform.position.y <= 0.15f) return true;
                break;
            case ShootDirection.RIGHT:
                if (nextPos.x <= -Globals.Instance.movePaceHorizontal && transform.position.x == 0) return true;
                break;
        }
        return false;
    }

    private TweenBase _moveBossTween;
    void MovePlayer()
    {
        currentState = PersonState.MOVING;
        switch (movementList[0])
        {
            case "UP": _moveBossTween = Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseInOutStrong, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "DOWN":
                if(!isAtEdge)
                {
                    Vector2 nextPos = nextPosition + new Vector3(0, -1f, 0);
                    HandleTweenMovingDownStarted();
                    //Tween.Position(gameObject.transform, nextPos, tweenDuration/2, 0, Tween.EaseInOutStrong, Tween.LoopType.None);
                    Invoke("HandleTweenMovingDownFinished", .5f);
                }
                else
                {
                    _moveBossTween = Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
                }
                break;
            case "RIGHT": _moveBossTween = Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;
            case "LEFT": _moveBossTween = Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenFinished); break;

            default:
                break;
        }
    }

    protected override void HandleTweenMovingDownStarted()
    {
        if(boxCollider.activeSelf) boxCollider.SetActive(false);
        currentState = PersonState.MOVING;
        IsFreeFall = true;
        Invoke("EnableCollider" , .4f);
    }

    private void EnableCollider()
    {
        //if (boxCollider != null)
        //{
        boxCollider.SetActive(true);
        //}
        Debug.Log("Turned Colldier On!");
    }
    protected override void HandleTweenMovingDownFinished()
    {
        if (movementList.Count > 0) movementList.Remove(movementList[0]);
        if (personIndex == 0) PlayerFinishedMoving?.Invoke();

        if(isAtEdge) Tween.Position(transform, lastPosition, Globals.Instance.tweenDuration, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, HandleTweenBossMovementFinished);
        else
        {
            if (!CheckPlatformUnderneath()) IsFreeFall = true; 
            else IsFreeFall = false;
        }

        currentState = PersonState.IDLE;
    }

    //Change later the name of the method
    protected override void HandleTweenStarted()
    {
        currentState = PersonState.MOVING;
    }

    //Called when move is finished
    protected override void HandleTweenFinished()
    {
        //Remove The move
        if (movementList.Count > 0) movementList.Remove(movementList[0]);                       
        //Only Tim triggers this event which gives signal to the level manager to reduce moves
        if (personIndex == 0 && !hasInteracatedWithSlide) PlayerFinishedMoving?.Invoke();

        if (isAtEdge) Tween.Position(transform, lastPosition, Globals.Instance.tweenDuration, 0, Tween.EaseInOutStrong, Tween.LoopType.None, null, HandleTweenBossMovementFinished);
        else
        {
            Invoke("CheckPlatform", 0.1f);
        }
        currentState = PersonState.IDLE;
    }

    private void CheckPlatform()
    {
        if (!CheckPlatformUnderneath()) IsFreeFall = true;
        else currentState = PersonState.IDLE; // OVO
    }
    private void HandleTweenBossMovementFinished()
    {
        isAtEdge = false;
    }

    private void HandleTweenSlideMoveFinished()
    {
        if (!CheckPlatformUnderneath())  IsFreeFall = true; 
        hasInteracatedWithSlide = false;
        currentState = PersonState.IDLE;

    }
    
    /// <summary>
    /// Calculates the next position where to move player.
    /// </summary>
    /// <param name="str"> Name of where to move.</param>
    /// <returns> Vector3 position of the position to move player. </returns>
    private Vector3 GetMovement(string str)
    {
        switch (str)
        {
            case "UP": isMovingDown = false; return transform.position + Globals.Instance.upVector;
            case "DOWN": isMovingDown = true; return transform.position + Globals.Instance.downVector;
            case "RIGHT": isMovingDown = false; return transform.position + Globals.Instance.rightVector;
            case "LEFT": isMovingDown = false; return transform.position + Globals.Instance.leftVector;

            default:
                isMovingDown = false; return transform.position;
        }
    }

    /// <summary>
    /// Shoots the ray down to check if there is platform underneath player.
    /// </summary>
    /// <returns>True if there is platform underneath, false no platforrm underneath. </returns>
    private bool CheckPlatformUnderneath()
    {    
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y/2 + +0.1f, 1 << 8);
        //Debug.DrawLine(transform.position, transform.position - new Vector3(0,  transform.localScale.y / 2 + +0.1f, 0) , Color.red);
        if (hit) return true;
        return false;
    }

    private bool ValidateBoundary(Vector2 positionToCheck)
    {
        //First check 
        switch (movementList[0])
        {
            /*case "UP": // Raise Death flag 
                if (nextPosition.y > Globals.Instance.verticalBoundary) return false;
                return true;
            case "DOWN": // Raise Death flag 
                if (nextPosition.y < -vGlobals.Instance.verticalBoundary) return false;
                return true;*/
            case "RIGHT":
                if (positionToCheck.x > Globals.Instance.horizontalBoundary) return false;
                return true;
            case "LEFT":
                if (positionToCheck.x < -Globals.Instance.horizontalBoundary) return false;
                return true;

            default: return true;
        }
    }

    private bool ValidateVerticalBoundary(Vector2 movement)
    {
        if (movement.y >= Globals.Instance.verticalBoundary || movement.y <= -Globals.Instance.verticalBoundary) return true;
        return false;
    }
    
    private BasePlatform GetPlatformUnderneath()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, transform.localScale.y / 2 + +0.1f, 1 << 8);
        if (hit)
        {
            BasePlatform platform = hit.collider.gameObject.GetComponent<BasePlatform>();
            return platform;
        }

        return null;
        
    }

    private void SubscribeBossMovement()
    {
        StartCoroutine("_MoveBossPlayerCoroutine");
    }
    
    public void SlimeStuck()
    {
        OnSlimeStuck?.Invoke();
    }
    
    #endregion

    #region INTERFACE IMPLEMENTATION

    public void DestroyObject()
    {
        if(!isProtected) Death("DestroyObject");
    }

    #endregion

    #region CHARACTER INTERACTIONS

    void ActivateShield()
    {
        StartCoroutine(_ActivateShieldCoroutine());
    }

    void OnReviveClick()
    {
        gameObject.transform.position = initialPosition; //Vector2.zero;
        currentState = PersonState.IDLE;
        isDeadFromFall = false;
    }
    
    public override void Death(string txt)
    {
        if(InputManager.Instance.isControllable)
        {
            InputManager.Instance.UnControlPlayer();
            LevelManager.Instance.ShowDefeatPanel(txt);

            base.Death(txt);

            if (!IsExtraLifeActive)
            {
                StopAllCoroutines();

                print("You have died");
            }
        }
    }

    public override void SlideMove(string move)
    {
        if (move == "RIGHT")
        {
            nextPosition = GetMovement("RIGHT");
            Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenSlideMoveFinished); 
        }
        else
        {
            nextPosition = GetMovement("LEFT");
            Tween.Position(gameObject.transform, nextPosition, Globals.Instance.tweenDuration, 0, Tween.EaseOut, Tween.LoopType.None, HandleTweenStarted, HandleTweenSlideMoveFinished);
        }
    }
    /*public override void SlimeInteraction()
    {
        InputManager.OnSwipedEvent -= AddMove;
        InputManager.OnSwipedEvent += ReduceTurn;
    }

    private void ReduceTurn(string str)
    {
        waitForTurns--;
        if(waitForTurns <= 0)
        {
            waitForTurns = 3;
            InputManager.OnSwipedEvent -= ReduceTurn;
            InputManager.OnSwipedEvent += AddMove;
        }
    }
    */



    #endregion

}
