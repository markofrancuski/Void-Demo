using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeartShake : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        LevelManager.OnHeartCollected += TriggerAnimation;
    }

    private void OnDisable()
    {
        LevelManager.OnHeartCollected -= TriggerAnimation;
    }

    public void TriggerAnimation(float param1, float param2)
    {
        _animator.SetTrigger("Collected");
    }
}
