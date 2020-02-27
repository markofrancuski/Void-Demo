using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class SlimePlatform : BasePlatform
{
    public int waitForTurnsAmount;
    [SerializeField] private List<int> numberOfMoves = new List<int>();

    private delegate void testdel();
    testdel del;

    public override void Interact(Person controller)
    {
        if (controller.isFrozen) return;

        controller.hasInteracatedWithSlide = false;
        FreezePlayer(controller);
        base.Interact(controller);
        InputManager.OnSwipedEvent += OnSwiped;
        //controller.SlimeInteraction();
        //FreezzePlayer(controller);
    }

    #region Unity Functions

    protected override void Start()
    {
        base.Start();
        waitForTurnsAmount = 3;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InputManager.OnSwipedEvent -= OnSwiped;

    }

    #endregion

    private void FreezePlayer(Person controller)
    {
        //Unsubscribe player from the InputManager
        controller.FreezePlayer();
        controller.UnsubscribePerson();
        numberOfMoves.Add(waitForTurnsAmount);

        int indexList = numberOfMoves.Count - 1;
        PlayerController pc = (PlayerController) controller;

        del = pc.SlimeStuck;
        Timing.RunCoroutine(_WaitForSwipes(indexList, controller).
            CancelWith(gameObject), $"Freeze{controller.personIndex}");
    }

    private IEnumerator<float> _WaitForSwipes(int index, Person controller)
    {
        //Debug.Log("Waiting for swipes! ");
        while(numberOfMoves[index] > 0) yield return Timing.WaitForOneFrame;

        controller.SubscribePerson();
        controller.UnFreezePlayer();
        Timing.KillCoroutines($"Freeze{controller.personIndex}");
        InputManager.OnSwipedEvent -= OnSwiped;
        del = null;
        if (IsListEmpty()) numberOfMoves.Clear();

    }

    //Subscribes to the input manager to listen for swipes
    private void OnSwiped(string str)
    {
        if (numberOfMoves.Count <= 0) return;

        del();

        Debug.Log("Reducing all number of moves");
        for (int i = 0; i < numberOfMoves.Count; i++)
        {
            numberOfMoves[i]--;
        }
    }

    private bool IsListEmpty()
    {
        for (int i = 0; i < numberOfMoves.Count; i++)
        {
            if (numberOfMoves[i] > 0) return false;
        }
        return true;
    }
}
