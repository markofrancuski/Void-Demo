using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//State Machine
public enum PersonState { IDLE, MOVING, DEAD, INTERACTING, STUNNED };

public class Person : MonoBehaviour
{
    public event UnityAction OnPlayerDeath;

    #region COMPONENTS
    [Header("SUPER-CLASS")]
    [Header("COMPONENTS")]
    [SerializeField] protected GameObject boxCollider;

    #endregion

    #region PERSON VARIABLES

    public int personIndex;
    public bool PlayerWon = false;

    public int Team;

    public bool IsStunned;

    public bool hasInteracatedWithSlide;
    public bool isReadyToDestroy;

    #endregion

    #region MOVEMENT VARAIBLES
    [Header("MOVEMENT VARAIBLES")]
    public bool ToogleMovementVariables = true;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    [SerializeField] protected List<string> movementList;

    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public  Vector3 initialPosition;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public PersonState currentState;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public bool isDeadFromFall;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public bool isMovingDown;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    [SerializeField] private bool isFreeFall;
    public bool IsFreeFall {
        get { return isFreeFall; }
        set {
            isFreeFall = value;

            if (value)
            {
                //Cast ray cast down
                //Check Distance between current position and the first platform below if its more then 1. => Death();

                RaycastHit2D[] hits;

                hits = Physics2D.RaycastAll(transform.position, Vector2.down, 20f, 1 << 8);

                CheckPlayerFalling(hits);
            }
        }
    }

    //Movement 
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public Vector3 nextPosition;
    [HidePropertyDrawer("ToogleMovementVariables", true)]
    public bool isMovementReversed;
    #endregion

    #region MOVEMENT FUNCTIONS

    void CheckPlayerFalling(RaycastHit2D[] hits)
    {
        //Is there platforms under the player
        if (hits.Length > 0)
        {
            //Debug.Log("Globals.Instance.movePaceHorizontal * 2: " + Globals.Instance.movePaceHorizontal * 2);

            //If there is more then one platform => raycast will hit the direct platform that is player moving from => get platform in grid under
            if (hits.Length > 1 && isMovingDown)
            {
                //Debug.Log("hits[1].distance: " + hits[1].distance);
                if (hits[1].distance >= Globals.Instance.movePaceHorizontal * 2) isDeadFromFall = true; 
                else isDeadFromFall = false;
                isMovingDown = false;
            }
            //There is only one platform under => Jumped up or on sides check first platform under player
            else
            {
                //Debug.Log("hits[0].distance: " + hits[0].distance);
                if (hits[0].distance >= Globals.Instance.movePaceHorizontal * 2) isDeadFromFall = true; 
                else isDeadFromFall = false;
            }

        }
        else
        {
            //No platform under => player will die upon landing
            isDeadFromFall = true;
        }
    }

    /// <summary>
    /// Changes the boolean value so you if you fall down and its true you die.
    /// </summary>
    public void EnterInFreeFall()
    {
        IsFreeFall = true; /*transform.parent = null;*/
    }

    public virtual void SlideMove(string move)
    {

    }
    #endregion

    #region UNITY FUNCTIONS
    // Start is called before the first frame update
    public virtual void Start()
    {
        //Debug.Log($"Person start! {gameObject.name} ");
        initialPosition = gameObject.transform.position;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (InputManager.Instance.isControllable)
        {
            if (other.CompareTag("Projectile")) { Death("Projectile"); return; };
            if (other.CompareTag("AI")) { Death("Moving Enemy"); return; };
            //int team = other.GetComponent<Projectile>().Team;
            //if (team != Team)
            //{
            //    Debug.Log($"GO from team: {team} has stunned GO from team: {Team}");
            //    Stun();
            //} 
            //else  Debug.Log($"Cannot stun yourself!");
        }
    }

    #endregion
    
    public virtual void Death(string txt)
    {
        Debug.Log("Death from: " + txt);
        currentState = PersonState.DEAD;
    }

    protected virtual void HandleTweenStarted()
    {
        currentState = PersonState.MOVING;
    }

    protected virtual void HandleTweenFinished()
    {

    }

    protected virtual void HandleTweenMovingDownStarted()
    {
        IsFreeFall = true;
        boxCollider.SetActive(false);
    }

    protected virtual void HandleTweenMovingDownFinished()
    {
        boxCollider.SetActive(true);

    }

    protected virtual void Stun()
    {        
        if(!IsStunned) StartCoroutine(_StunPersonCoroutine());
    }

    public bool isFrozen;
    public void FreezePlayer() { isFrozen = true; ClearList(); }
    public void UnFreezePlayer() => isFrozen = false;

    protected void AddMove(string movement)
    {
        /*if (!isMovementReversed) movementList.AddLast(movement);
        else movementList.AddLast(GetReversedMovement(movement) );*/
        if(!isFrozen) movementList.Add(movement);//!isFreeFall && 
    }
    protected void ClearList() => movementList.Clear();
    public void ParentPerson(Transform parent)
    {
        gameObject.transform.SetParent(parent);
    }

    public void UnParentPerson()
    {
        gameObject.transform.SetParent(null);
    }

    /// <summary>
    /// Unsubscribes the player to listening for inputmanager
    /// </summary>
    public void UnsubscribePerson() { InputManager.OnSwipedEvent -= AddMove; }
    /// <summary>
    /// Subscribes the player to listening for inputmanager
    /// </summary>
    public void SubscribePerson() { InputManager.OnSwipedEvent += AddMove; }

    #region HELPER FUNCTIONS

    /// NE RADI REVERSE JER NIJE POZVAN NIGDE
    private string GetReversedMovement(string movement)
    {
        switch (movement)
        {
            case "RIGHT": return "LEFT";
            case "LEFT": return "RIGHT";
            case "UP": return "DOWN";
            case "DOWN": return "UP";
            default:
                return "";
        }
    }
    
    #endregion

    #region COROUTINES

    IEnumerator _StunPersonCoroutine()
    {
        IsStunned = true;
        currentState = PersonState.STUNNED;

        yield return new WaitForSeconds(3f);

        currentState = PersonState.IDLE;
        IsStunned = false;

    }

    #endregion
}
