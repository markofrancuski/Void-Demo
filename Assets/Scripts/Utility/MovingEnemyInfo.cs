using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovingEnemyInfo
{
    public Vector2 SpawnPosition;
    public List<Vector2> MovingPaths;

    public MovingEnemyInfo(Vector2 SpawnPosition, List<Vector2> MovingPaths)
    {
        this.SpawnPosition = SpawnPosition;
        this.MovingPaths = MovingPaths;
    }

    public override string ToString()
    {
        return $"MovingEnemyInfo, SP:{SpawnPosition}, Paths: {MovingPaths.Count}";
    }
}
