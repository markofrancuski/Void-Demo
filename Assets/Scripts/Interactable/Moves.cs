using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moves : Interactable
{
    [SerializeField] private int movesToAdd;
    public void SetUpPowerUp(int number)
    {
        movesToAdd = number;
    }

    public override void Interact()
    {
        Debug.Log("Moves power up is interacting!");
        LevelManager.Instance.AddMoves(movesToAdd);
        base.Interact();
    }
}