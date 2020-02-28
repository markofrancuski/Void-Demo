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
        if (_particleEffect != null)
        {
            GameObject go = Instantiate(_particleEffect, transform.position, Quaternion.identity);
        }
        LevelManager.Instance.AddMoves(movesToAdd);
        base.Interact();
    }
}