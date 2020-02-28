using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Interactable
{

    public override void Interact()
    {
        if (_particleEffect != null)
        {
            GameObject go = Instantiate(_particleEffect, transform.position, Quaternion.identity);          
        }

        LevelManager.Instance.HeartCollected();
        base.Interact();
    }

}
