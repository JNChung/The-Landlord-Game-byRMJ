using System.Collections.Generic;
using UnityEngine;

public class Character : ICanMove
{
    public float Hp;
    public Vector3Int Position;
    public int Speed;
    public Character(float hp, Vector3Int pos, int speed)
    {
        this.Hp = hp;
        this.Position = pos;
        this.Speed = speed;
    }
    public IPath GetCurrentTile()
    {
        return StaticSceneData.TileManager.GetTileByCoordinate(Position);
    }
    public IEnumerable<PathData> CanMoveTiles(int moveProp)
    {
        return MovementAlgorithm.NormalMove(this, StaticSceneData.TileManager);
    }

    public int GetSpeed()
    {
        return Speed;
    }
}

//規格

