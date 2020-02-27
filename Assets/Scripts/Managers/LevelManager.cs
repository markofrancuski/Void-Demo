using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pixelplacement;
using UnityEngine.SceneManagement;
using MEC;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    #region EVENTS
    public delegate void BossStateEventHandler();
    public static event BossStateEventHandler BossKilled;

    public static event BossStateEventHandler BossChange;

    public delegate void ResetLevelEventHandler();
    public static event ResetLevelEventHandler ResetLevel;

    public delegate void GivePlayerWinEventHandler();
    public static event GivePlayerWinEventHandler GivePlayerWin;

    public delegate void OnLevelBuildingFinishedEventHandler();
    public static event OnLevelBuildingFinishedEventHandler LevelFinishedBuilding;

    public static event UnityAction<float, float> OnHeartCollected;
    #endregion

    public bool IsBossLevel;
    public int bossLevelIndex;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private GameObject timPrefab;

    public Boss bossScript;
    [SerializeField] private Image _bossHealthImage;

    #region COMPONENT REFERENCES

    [SerializeField] private Animator canvasAnimator;
    //public GameObject levelGO;

    #endregion

    #region CURRENT LEVEL SETTINGS
    [Header("Current Level Settings")]
    [SerializeField] private int currentChapter;

    [SerializeField] private int maxMoves;
    public int MaxMoves 
    {
        get { return maxMoves; }
        set 
        {
            maxMoves = value;
            tempMaxMoves = value;
            UpdateMoves();
        }
    }

    [SerializeField] private int tempMaxMoves;

    [SerializeField] private int heartToCollect;
    public int HeartToCollect 
    {
        get { return heartToCollect; }
        set 
        {
            heartToCollect = value;
            tempHeartToCollect = value;
            _fillIncrementMovePos = -0.57f / HeartToCollect;
            _fillIncrement = 1.06641f / tempHeartToCollect;
            UpdateHearts();
        }
    }
    [SerializeField] private int tempHeartToCollect = 0;
    #endregion

    #region UI REFERENCES
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI heartsText;
    [SerializeField] private TextMeshProUGUI currentChapterText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI defeatText;
    [SerializeField] private GameObject nextLevelButton;
    #endregion

    public int Players;
    [SerializeField] private float _fillIncrement;
    [SerializeField] private float _fillIncrementMovePos;

    #region Unity Functions

    private void Awake()
    {
        Instance = this;
        if(IsBossLevel)
        {
            GameObject go = Instantiate(bossPrefab);
            bossScript = go.GetComponent<Boss>();
            bossScript.direction = ShootDirection.DOWN;
            go.transform.position = new Vector3(0, Globals.Instance.movePaceVertical, 0);
        }
    }

    private void Start()
    {

        if (IsBossLevel)
        {
            Invoke("StartAfterLevelSpawn", .1f);
        }
        else
        {

            Globals.Instance.SpawnCurrentLevel(); 
            tempMaxMoves = maxMoves;
            UpdateMoves();
            UpdateHearts();

            Invoke("SceneReady", .2f); // Ovo
        }
        currentChapterText.SetText("Chapter:" + Globals.Instance.currentChapter);
        currentLevelText.SetText("Level:" + Globals.Instance.currentLevel);
    }

    private void StartAfterLevelSpawn()
    {

        GameObject Tim = Instantiate(timPrefab);
        Tim.transform.localPosition = Vector3.zero;
        Debug.Log("Tim position: " + Tim.transform.localPosition);

        //levelGO = Instantiate(BossLevels[0]);
        _bossHealthImage.gameObject.SetActive(true);
        //Spawn Tim at center;

        Invoke("SceneReady", .2f); // Ovo
    }

    private void OnEnable()
    {
        //InputManager.OnSwipedEvent += ReduceMoves;
        if (!IsBossLevel)
        {
            PlayerController.PlayerFinishedMoving += ReduceMoves;
            Globals.Instance.ChangeTweenSpeed(false);
        }
        else
        {
            Boss.BossTookDamage += UpdateBossHealth;
            Globals.Instance.ChangeTweenSpeed(true);
        }
        ResetLevel += ResetCurrentLevel;
        
        //tempMaxMoves = maxMoves;
        //tempHeartToCollect = heartToCollect;

    }

    private void OnDisable()
    {
        //InputManager.OnSwipedEvent -= ReduceMoves;
        if (!IsBossLevel)
        {
            PlayerController.PlayerFinishedMoving -= ReduceMoves;
        }
        else Boss.BossTookDamage -= UpdateBossHealth;
        ResetLevel -= ResetCurrentLevel;
    }

    #endregion

    #region Helper Functions

    //Public Functions
    public void SceneReady()
    {

        Globals.Instance.isSceneReady = true;
        InputManager.Instance.ControlPlayer();
        if (IsBossLevel) LevelFinishedBuilding?.Invoke();
    }
    
    /// <summary>
    /// Gets called when you swipe to move your character
    /// </summary>
    public void ReduceMoves()
    {
        tempMaxMoves--;
        UpdateMoves();
        if (tempMaxMoves <= 0 && tempHeartToCollect > 0)
        {   
            // You didn't pass the level => Out of moves!
            ShowDefeatPanel();
            // Invoke option to reset level
        }
    }
    
    /// <summary>
    /// Gets called when you collect heart in current level
    /// </summary>
    public void HeartCollected()
    {

        //Debug.Log("Heart Collected!");
        tempHeartToCollect--;
        //Notify player filler to fill up body.
        OnHeartCollected?.Invoke(_fillIncrement, _fillIncrementMovePos);
        if (tempHeartToCollect <= 0 && tempMaxMoves >= 0)
        {
            LevelFinished();
            //Switch to the next level/chapter
        }
        UpdateHearts();
    }

    public void CheckBossFinished()
    {      
        if (bossLevelIndex == 2)
        {
            LevelFinished();
        }
        else
        {
            Globals.Instance.isSceneReady = false;
            InputManager.Instance.UnControlPlayer();
            bossLevelIndex++;


            BossKilled?.Invoke();

            bossScript.EnableBoss();

            Invoke("SceneReady", .5f);
        }
    }
   
    /// <summary>
    /// Add moves when player picks up the power up.
    /// </summary>
    public void AddMoves(int number)
    {
        tempMaxMoves += number;
        UpdateMoves();
    }
    [SerializeField] private Person[] players = new Person[2];
    public void ReverseMovement(int index)
    {
        //To reverse Tim controlls index is 0, for Annie its 1.
        if(!players[index].isMovementReversed) players[index].isMovementReversed = true;
        else players[index].isMovementReversed = false;
        Debug.Log($"Reverse: {players[index].isMovementReversed}");
    }
    public void SubscribePlayer(int index, Person person)
    {
        players[index] = person;
    }

    //Private Functions
    private void LevelFinished()
    {
        GivePlayerWin?.Invoke();

        InputManager.Instance.UnControlPlayer();
        canvasAnimator.SetTrigger("Victory");

        if (Globals.Instance.IsEnd()) nextLevelButton.SetActive(false);
        else nextLevelButton.SetActive(true);
    }
    private void ResetCurrentLevel()
    {
        SceneSwitcher.Instance.LoadLevel(0.5f, Globals.Instance.GetCurrentLevelSceneName());
    }

    #endregion

    #region UI functions

    private void UpdateMoves() => movesText.SetText(tempMaxMoves.ToString());
    public void UpdateHearts() => heartsText.SetText(tempHeartToCollect.ToString());

    public void OnResetCurrentLevelClicked()
    {
        canvasAnimator.SetBool("WaitForLevelSetup", true);
        SceneSwitcher.Instance.LoadLevel(2f, Globals.Instance.GetCurrentLevelSceneName());
        //ResetLevel?.Invoke();
    }

    private bool isClicked;
    /// <summary>
    /// Loads up the Main Menu Scene
    /// </summary>
    public void OnBackButtonClicked()
    {
        if (isClicked) return;
        isClicked = true;
        //Load Main Menu scene
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Shows up the animation and loads next level or chapter.
    /// </summary>
    public void OnNextLevelClicked()
    {
        canvasAnimator.SetBool("WaitForLevelSetup", true);
        Globals.Instance.GoToNextLevelInChapter();
        //Transition in black
        //Async Load
    }

    /// <summary>
    /// Shows the defeat panel with provided text displayed
    /// </summary>
    public void ShowDefeatPanel(string txt)
    {    
        defeatText.SetText("Died from " + txt);
        canvasAnimator.SetTrigger("Defeat");
    }

    /// <summary>
    /// Shows the defeat panel with 'Out of the moves' text displayed
    /// </summary>
    public void ShowDefeatPanel()
    {
        InputManager.Instance.UnControlPlayer();
        defeatText.SetText("Out of the moves!");
        canvasAnimator.SetTrigger("Defeat");
    }
    
    public void UpdateBossHealth(int maxHealth , int amount)
    {
        //print("maxHealth: " + maxHealth + " amount: " + amount + " fillAmount: " + amount / maxHealth);
        _bossHealthImage.fillAmount =  (float) amount / maxHealth;
    }
    #endregion

}
