using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class DoubleLevel : Singleton<DoubleLevel>
{
    private Vector3 frontPosition;
    private Vector3 backPosition;

    public Transform[] levels;
    public int activeLevelIndex;

    #region Unity functions

    private void Awake()
    {
        PlayerController.OnMoved += OnSwipe;
    }

    private void Start()
    {
        Globals.Instance.ChangeTweenSpeed(false);
        backPosition = frontPosition = Vector3.zero;
        frontPosition.z = -1;
        backPosition.z = 0;

        levels[0].transform.position = frontPosition;
        levels[1].transform.position = backPosition;
    }

    private void OnEnable()
    {
        //InputManager.OnSwipedEvent += OnSwipe;
       
    }

    private void OnDisable()
    {
        //InputManager.OnSwipedEvent -= OnSwipe;
        PlayerController.OnMoved -= OnSwipe;
    }

    #endregion

    public void OnSwipe()
    {
        levels[activeLevelIndex].transform.position = backPosition;
        levels[activeLevelIndex].transform.gameObject.SetActive(false);
        activeLevelIndex++;
        if(activeLevelIndex >= levels.Length)
        {
            activeLevelIndex = 0;
        }
        levels[activeLevelIndex].transform.position = frontPosition;
        levels[activeLevelIndex].transform.gameObject.SetActive(true);
    }

    public void Switch()
    {
        Debug.Log("Portal Activated 1");
        activeLevelIndex++;
        if (activeLevelIndex >= levels.Length)
        {
            activeLevelIndex = 0;
        }
    }
    
}
