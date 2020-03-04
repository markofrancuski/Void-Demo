using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemyCollision : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            collision.gameObject.transform.parent.GetComponent<IDestroyable>().DestroyObject();
        }
    }
}
