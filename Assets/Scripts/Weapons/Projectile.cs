﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D rigidBody;

    [SerializeField] private Vector2 direction;

    public int Team;

    public void SetUpProjectile(int team, Vector2 dir, float speed = 150f)
    {
        Team = team;
        direction = dir;
        _speed = speed;
    }

    private void FixedUpdate()
    {
        
        rigidBody.velocity = direction * _speed * Time.deltaTime;

    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
