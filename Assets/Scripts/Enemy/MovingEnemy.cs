using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Pixelplacement;

public class MovingEnemy : MonoBehaviour
{
    [Header("Starting Position")]
    [SerializeField] Vector2 spawnPosition;

    [Header("Wait Timer before next move")]
    [Range(0.5f , 1)]
    [SerializeField] private float minTime;
    [Range(1, 3)]
    [SerializeField] private float maxTime;
    [SerializeField] private float waitTimer;

    [Header("Move Path Settings")]
    [SerializeField] private Vector2[] movePath;
    [SerializeField] private int moveIndex = 0;

    #region Unity Functions

    private void Start()
    {
        //Position the Enemy on a platform.
        //gameObject.transform.position = new Vector3(spawnPosition.x * Globals.Instance.movePaceHorizontal, (spawnPosition.y * Globals.Instance.movePaceVertical) + 0.05f, 0);
        waitTimer = Random.Range(minTime, maxTime);
        if(movePath.Length> 0) Timing.RunCoroutine(_MoveCoroutine().CancelWith(gameObject));

    }

    #endregion

    #region Coroutines

    private IEnumerator<float> _MoveCoroutine()
    {
        Debug.Log("Starting Coroutine");
        //yield return Timing.WaitForSeconds(2f);

        while(true)
        {
            yield return Timing.WaitForSeconds(waitTimer);
            Tween.Position(transform, transform.position + GetVectorMove(), Globals.Instance.tweenDuration, 0 );
            Timing.WaitForSeconds(Globals.Instance.tweenDuration);
        }

    }

    #endregion

    private Vector3 GetVectorMove()
    {
        Vector3 move = new Vector2();
        move.x += movePath[moveIndex].x * Globals.Instance.movePaceHorizontal;
        move.y += movePath[moveIndex].y * Globals.Instance.movePaceVertical;

        moveIndex++;
        if (moveIndex >= movePath.Length) moveIndex = 0;

        waitTimer = Random.Range(minTime, maxTime);

        return move;

    }

}
