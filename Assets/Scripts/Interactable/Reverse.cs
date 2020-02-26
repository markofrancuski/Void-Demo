using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reverse : MonoBehaviour
{

    public void Interact(string name)
    {
        //If its Tim => Reverse movement to Annie.
        int index = 0;
        if (name == "Annie(Clone)") { index = 1; }
        LevelManager.Instance.ReverseMovement(index);

        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) Interact(other.name);
    }
}
