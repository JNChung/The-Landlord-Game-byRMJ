using System.Collections.Generic;
using UnityEngine;

public class Character : ICanMove
{
    public float Hp;
    public Vector3Int Position;
    public int Speed;
    MoveWay moveWay;
    public Character(float hp, Vector3Int pos, int speed, MoveWay moveWay = null)
    {
        this.Hp = hp;
        this.Position = pos;
        this.Speed = speed;
        if (moveWay == null) moveWay = MovementAlgorithm.NormalMove;
    }
    public IPath GetCurrentTile(IMapProvider mapProvider)
    {
        return mapProvider.GetTileByCoordinate(Position);
    }
    public IEnumerable<PathData> CanMoveTiles(IMapProvider mapProvider)
    {
        return moveWay(this, mapProvider);
    }

    public int GetSpeed()
    {
        return Speed;
    }
}

//規格

