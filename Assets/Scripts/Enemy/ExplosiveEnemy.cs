using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class ExplosiveEnemy : MonoBehaviour
{
    [SerializeField] private bool hasBeenTouched;
    public float exploadTimer = 3f;

    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private Vector2 movePosition;
    private Vector3 scaleVector = new Vector3(0, 0.1f, 0);
    private Vector3 moveVector = new Vector3(0, 0.05f, 0);

    [SerializeField] private List<BasePlatform> _surrondingPlatforms;
    public int PlatformIndex;

    #region Unity Functions

    private void Start()
    {

        //Get surronding platforms
        Level lvl = Globals.Instance.levelGO.GetComponent<Level>();
        
        //Ray Up
        int index = PlatformIndex - lvl.GridSize;
        if (index >= 0)
        {
            if (lvl.transform.GetChild(index).GetChild(1).gameObject.activeInHierarchy)
                _surrondingPlatforms.Add(lvl.transform.GetChild(index).GetChild(1).GetComponent<BasePlatform>());
        }
        
        //Ray Down
        index = PlatformIndex + lvl.GridSize;
        if(index <= Mathf.Pow(lvl.GridSize,2))
        {
            if (lvl.transform.GetChild(index).GetChild(1).gameObject.activeInHierarchy)
                _surrondingPlatforms.Add(lvl.transform.GetChild(index).GetChild(1).GetComponent<BasePlatform>());
        }

        //Ray Left
        index = PlatformIndex - 1;
        if (index % 5 != 0 && index >= 0)
        {
            if(lvl.transform.GetChild(index).GetChild(1).gameObject.activeInHierarchy)
                _surrondingPlatforms.Add( lvl.transform.GetChild(index).GetChild(1).GetComponent<BasePlatform>() );
        }
        
        //Ray Right
        index = PlatformIndex + 1;
        if (index % 4 != 0 && index < Mathf.Pow(lvl.GridSize, 2))
        {
            if (lvl.transform.GetChild(index).GetChild(1).gameObject.activeInHierarchy)
                _surrondingPlatforms.Add(lvl.transform.GetChild(index).GetChild(1).GetComponent<BasePlatform>());
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, Vector2.up * Globals.Instance.movePaceVertical, Color.red);

        Debug.DrawRay(transform.position, Vector2.down * Globals.Instance.movePaceVertical, Color.red);

        Debug.DrawRay(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.right * Globals.Instance.movePaceHorizontal, Color.red);
        Debug.DrawRay(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.left * Globals.Instance.movePaceHorizontal, Color.red);
#endif
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTouched)
        {
            Debug.Log("Starting Timer ");
            hasBeenTouched = true;
            //Invoke("Expload", exploadTimer);
            StartTicking();
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    #endregion

    private IEnumerator<float> _StartTimer()
    {
        //Wait for 'X' seconds
        float tempTimer = exploadTimer;

        while(tempTimer > 0)
        {
            Debug.Log("Ticking timer: " + tempTimer);
            gameObject.transform.localScale += scaleVector;
            gameObject.transform.localPosition += moveVector;
            //Scale Enemy
            yield return Timing.WaitForSeconds(1f);         
            tempTimer -= 1f;
        }
        
        //Expload
        //Destroy all surrounding platforms(UP, DOWN, LEFT, RIGHT)
        Expload();

    }

    /// <summary>
    /// Run the Coroutine that ticks every second and scales the enemy, after 3rd tick it explodes;
    /// </summary>
    public void StartTicking() => Timing.RunCoroutine(_StartTimer().CancelWith(gameObject));

    void Expload()
    {
        //Play Particle System

        Debug.Log($"Enemy {gameObject.name} Exploaded!");
        foreach (var platform in _surrondingPlatforms)
        {
            platform.DestroyObject();
        }

        Destroy(gameObject);
    }

    public void SetupEnemy(Vector2 sp)
    {
        //spawnPosition = sp;
        transform.position = sp;
    }
}
