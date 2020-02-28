using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ParallexChild : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [Space]
    [SerializeField] private float _checkPos;
    [SerializeField] private float _resetPos;
    [Space]
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private float moveSpeed;
    private void Start()
    {
        _rigidbody.velocity = moveDirection * moveSpeed;    
    }

    public void ResetChild()
    {
        transform.position = new Vector2(_resetPos, 0);
    }

    private void Update()
    {
        if (transform.position.x >= _checkPos) ResetChild();
    }
}
