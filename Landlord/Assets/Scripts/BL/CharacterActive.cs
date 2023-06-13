using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : ICanMove
{
    public float Hp;
    public Vector3Int Position;
    public int moveAbility;
    MoveWay moveWay;
    float deviation;//距離誤差，低於此值視為相等
    public Character(float hp, Vector3Int pos, int speed, MoveWay moveWay = null)
    {
        this.Hp = hp;
        this.Position = pos;
        this.moveAbility = speed;
        if (moveWay == null) moveWay = MovementAlgorithm.NormalMove;
    }
    #region ICanMove
    public IPath GetCurrentTile(IMapProvider mapProvider)
    {
        return mapProvider.GetTileByCoordinate(Position);
    }
    public IEnumerable<PathData> CanMoveTiles(IMapProvider mapProvider)
    {
        return moveWay(this, mapProvider);
    }
    public int GetHowFarItCanBe()
    {
        return moveAbility;
    }
    public float GetPerformanceSpeed()
    {
        return 1;
    }

    public bool Seek(PathData pathData, float deltaTime, ref Vector3 newPos)
    {
        float speed = GetPerformanceSpeed();
        return false;
    }

    internal void MoveTo(Vector3 position)
    {
        throw new NotImplementedException();
    }

    #endregion
}

//規格

