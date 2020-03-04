using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovingEnemyInfo
{
    public Vector2 SpawnPosition;
    public List<ShootDirection> MovingPaths;

    public MovingEnemyInfo(Vector2 SpawnPosition, List<ShootDirection> MovingPaths)
    {
        this.SpawnPosition = SpawnPosition;
        this.MovingPaths = MovingPaths;
        Debug.Log($"MovingEnemyInfo, SP:{SpawnPosition}, Paths: {MovingPaths.Count}");
    }

    public override string ToString()
    {
        return $"MovingEnemyInfo, SP:{SpawnPosition}, Paths: {MovingPaths.Count}";
    }
}
