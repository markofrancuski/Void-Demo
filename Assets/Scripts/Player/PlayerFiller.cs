using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFiller : MonoBehaviour
{
    //private float _startPosX;

    private void OnEnable()
    {
        LevelManager.OnHeartCollected += FillSprite;
    }

    private void OnDisable()
    {
        LevelManager.OnHeartCollected -= FillSprite;
    }

    void FillSprite(float fillAmount, float movePosX)
    {
        Vector3 newScale = transform.localScale;
        newScale.y += fillAmount;

        Vector3 newPos = transform.localPosition;
        newPos.y += Mathf.Abs(movePosX);

        transform.localScale = newScale;
        transform.localPosition = newPos;
    }
}
