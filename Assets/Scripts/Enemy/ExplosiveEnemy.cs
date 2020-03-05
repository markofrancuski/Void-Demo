using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Pixelplacement;

public class ExplosiveEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _particleExplosion;
    [SerializeField] private bool hasBeenTouched;
    public float exploadTimer = 3f;

    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private Vector2 movePosition;
    private Vector3 scaleVector = new Vector3(0.1f, 0.1f, 0);
    private Vector3 moveVector = new Vector3(0, 0.05f, 0);

    [SerializeField] private List<BasePlatform> _surrondingPlatforms;
    public int PlatformIndex;

    #region Unity Functions

    private void Start()
    {

        //Get surronding platforms
        Level lvl = Globals.Instance.levelGO.GetComponent<Level>();

        //Current Platform
        if (PlatformIndex >= 0)
        {
            GameObject go = lvl.transform.GetChild(PlatformIndex).GetChild(1).gameObject;
            if (go.name == "Platform")
                _surrondingPlatforms.Add(go.GetComponent<BasePlatform>());
        }
        //Platform Up
        int index = PlatformIndex - lvl.GridSize;
        if (index >= 0)
        {
            GameObject go = lvl.transform.GetChild(index).GetChild(1).gameObject;
            if (go.name == "Platform")
                _surrondingPlatforms.Add(go.GetComponent<BasePlatform>());
        }

        //Platform Down
        index = PlatformIndex + lvl.GridSize;
        if(index <= Mathf.Pow(lvl.GridSize,2))
        {
            GameObject go = lvl.transform.GetChild(index).GetChild(1).gameObject;
            if (go.name == "Platform")
                _surrondingPlatforms.Add(go.GetComponent<BasePlatform>());
        }

        //Platform Left
        index = PlatformIndex - 1;
        if (index % 5 != 0 && index >= 0)
        {
            GameObject go = lvl.transform.GetChild(index).GetChild(1).gameObject;
            if (go.name == "Platform")
                _surrondingPlatforms.Add(go.GetComponent<BasePlatform>() );
        }

        //Platform Right
        index = PlatformIndex + 1;
        if (index % 4 != 0 && index < Mathf.Pow(lvl.GridSize, 2))
        {
            GameObject go = lvl.transform.GetChild(index).GetChild(1).gameObject;
            if (go.name == "Platform")
                _surrondingPlatforms.Add(go.GetComponent<BasePlatform>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTouched)
        {
            Debug.Log("Starting Timer ");
            hasBeenTouched = true;
            StartTicking();
        }
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    #endregion

    public void StartTicking()
    {
        Tween.LocalScale(transform, transform.localScale, transform.localScale + scaleVector * exploadTimer, exploadTimer, 0, null, Tween.LoopType.None, null, Expload);
    }

    void Expload()
    {
        //Play Particle System

        foreach (var platform in _surrondingPlatforms)
        {
            if(platform.gameObject.activeInHierarchy) platform.DestroyObject("Explosion");
        }

        Destroy(gameObject);
    }

    public void SetupEnemy(Vector2 sp)
    {
        //spawnPosition = sp;
        transform.position = sp;
    }
}
