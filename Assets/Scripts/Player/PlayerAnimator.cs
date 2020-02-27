using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        GetComponent<PlayerController>().OnSlimeStuck += PlayStuck;
        GetComponent<Person>().OnPlayerDeath += PlayDeath;
        
    }
    private void OnDisable()
    {
        GetComponent<PlayerController>().OnSlimeStuck -= PlayStuck;
        GetComponent<Person>().OnPlayerDeath -= PlayDeath;
    }

    private void PlayStuck()
    {
        _animator.SetTrigger("Stuck");
    }

    private void PlayDeath()
    {
        _animator.SetTrigger("Death");
    }
}
