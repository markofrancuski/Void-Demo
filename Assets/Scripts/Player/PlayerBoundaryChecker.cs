using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBoundaryChecker : MonoBehaviour
{
    public UnityAction OutOfBounds;

    [SerializeField] private float _yPosCheck;
    [SerializeField] private bool _isInvoked;

    private void OnEnable()
    {
        OutOfBounds += SetInvoke;
    }

    private void OnDisable()
    {
        OutOfBounds -= SetInvoke;
    }

    private void SetInvoke() => _isInvoked = true;

    // Update is called once per frame
    void Update()
    {
        if (!_isInvoked && (transform.position.y <= _yPosCheck || transform.position.x >= 3.2 || transform.position.x <= -3.2)) OutOfBounds?.Invoke();
    }

}
