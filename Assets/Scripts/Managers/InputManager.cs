﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class InputManager : Singleton<InputManager>
{

    #region INPUT
    [Header("INPUT")]
    private Vector2 startTouchPosition;
    private Vector3 swipeDirection;
    [SerializeField] private float swipeSensitivity;

    /// <summary>
    /// Boolean value if its true(Means that player is not dead and can check for input).
    /// </summary>
    public bool isControllable = false;
    #endregion

    #region SWIPE EVENT
    public delegate void OnSwiped(string str);
    //Subscribe methods form other scripts to listen this event => When triggered pass calls this function and pass the argument => subscribed methods will be called with the argument
    public static event OnSwiped OnSwipedEvent;

    //Fetch the PlayerState for checking input
    public delegate PersonState OnPlayerStateCheck();
    public static event OnPlayerStateCheck OnPlayerStateCheckEvent;

    public delegate void OnUnControlPlayerEventHandler();
    public static event OnUnControlPlayerEventHandler OnUnControlPlayer;

    #endregion

    [SerializeField] private bool pauseGame = false;
    // Update is called once per frame
    void Update() // Maybe fixedUpdate
    {
        if(Input.GetKeyUp(KeyCode.M))
        {
            pauseGame = !pauseGame;
            if (pauseGame) Time.timeScale = 0f;
            else Time.timeScale = 1f;
        }
        if (!isControllable) return;

#if UNITY_EDITOR
        if (OnPlayerStateCheckEvent.Invoke() != PersonState.DEAD) CheckEditorInput();
#else
            if (OnPlayerStateCheckEvent.Invoke() != PersonState.DEAD) CheckTouchInput();
#endif
    }

    public bool isPressed = false;
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 swipeDire;

    void CheckEditorInput()
    {

        if (isPressed)
        {
            if (Input.GetMouseButtonUp(0))
            {
                endPos = Input.mousePosition;

                swipeDire = (endPos - startPos).normalized;
                if (swipeDire.x >= swipeSensitivity) AddMovement("RIGHT");
                else if (swipeDire.x <= -swipeSensitivity) AddMovement("LEFT");
                else if (swipeDire.y >= swipeSensitivity) AddMovement("UP");
                else if (swipeDire.y <= -swipeSensitivity) AddMovement("DOWN");

                isPressed = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPressed = true;
                startPos = Input.mousePosition;
            }
        }
    }

    void CheckTouchInput()
    {
        if (Input.touchCount > 0)
        {
            //First touch => First finger that touched the screen.
            Touch touch = Input.GetTouch(0);

            //User start pressing the screen => Retreive the start position.
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            //User is no longer touching the screen => Get the position => Check the direction, 'Swiping'.
            if (touch.phase == TouchPhase.Ended)
            {
                swipeDirection = (touch.position - startTouchPosition).normalized;

                if (swipeDirection.x >= swipeSensitivity)
                {
                    AddMovement("RIGHT");
                }

                else if (swipeDirection.x <= -swipeSensitivity)
                {
                    AddMovement("LEFT");
                }

                else if (swipeDirection.y >= swipeSensitivity)
                {
                    AddMovement("UP");
                }

                else if (swipeDirection.y <= -swipeSensitivity)
                {
                    AddMovement("DOWN");
                }

            }
        }
    }

    void AddMovement(string str)
    {
        // Alert subscribed methods if its not null
        OnSwipedEvent?.Invoke(str);
    }

    /// <summary>
    /// Player is instantiated => start checking the input.
    /// </summary>
    public void ControlPlayer() => isControllable = true;

    /// <summary>
    /// Player is dead or he passed the level => Don't check input.
    /// </summary>
    public void UnControlPlayer()
    {
        OnUnControlPlayer?.Invoke();
        isControllable = false;      
    }

}
