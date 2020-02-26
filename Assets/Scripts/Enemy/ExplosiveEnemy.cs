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

    [SerializeField] Transform platformPosition;
    public int gridSize;
    public int platformIndex;

    #region Unity Functions

    private void Start()
    {
         /*platformPosition = LevelManager.Instance.levelGO.transform;
         gridSize = (int) Mathf.Sqrt(platformPosition.childCount);

        movePosition = new Vector3(platformPosition.GetChild(platformIndex).gameObject.transform.position.x, platformPosition.GetChild(platformIndex).gameObject.transform.position.y + 0.15f, 0);
        Debug.Log(platformPosition.GetChild(platformIndex).gameObject.name);
        Debug.Log(platformPosition.GetChild(platformIndex).gameObject.transform.position.x);
        Debug.Log(platformPosition.GetChild(platformIndex).gameObject.transform.position.y);
        //Position the Enemy on a platform.
        gameObject.transform.position = movePosition;*/
    }

    private void Update()
    {
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, Vector2.up * Globals.Instance.movePaceVertical, Color.black);

        Debug.DrawRay(transform.position, Vector2.down * -Globals.Instance.movePaceVertical, Color.black);

        Debug.DrawRay(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.right * Globals.Instance.movePaceHorizontal, Color.black);
        Debug.DrawRay(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.left * -Globals.Instance.movePaceHorizontal, Color.black);
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
        //Shoot RayCasts in all 4 direction 
        RaycastHit2D[] hits;

        //Raycast Up
        hits = Physics2D.RaycastAll(transform.position, Vector2.up, 2); // verticalSize
        foreach (RaycastHit2D hit in hits)
        {
            //|| hit.collider.CompareTag("Player")
            if (hit.collider.CompareTag("Platform")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position, Vector2.down, 2.3f); // verticalSize + 0,3f
        //Raycast Down
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.right, 1.125f); //horizontalSize
        //Raycast Right
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        hits = Physics2D.RaycastAll(transform.position + new Vector3(0f, -0.55f, 0f), Vector2.left, 1.125f); //horizontalSize
        //Raycast Left
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.CompareTag("Platform")) hit.collider.gameObject.GetComponent<IDestroyable>().DestroyObject();
        }

        Destroy(gameObject);
    }

    
}
