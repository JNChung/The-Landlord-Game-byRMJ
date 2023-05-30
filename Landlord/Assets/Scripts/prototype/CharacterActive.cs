using Ron.Base.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MovementAlgorithm;

public class Character : ICanMove
{
    public float Hp;
    public Vector3Int Position;
    public int Speed;
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
public interface ICanMove
{
    public IPath GetCurrentTile();
    public int GetSpeed();
}
//規格
public interface IMapProvider
{
    IPath GetTileByCoordinate(Vector3Int vector3Int);//地圖編輯器"那邊"要把這個功能實作出來
    ICollection<IPath> GetMap();
}
public class PathData
{
    public IPath Start;
    public IPath Current;
    public int Distance;
    public PathData(IPath start, IPath current, int distance)
    {
        this.Current = current;
        this.Start = start;
        this.Distance = distance;
    }
}
public class TileData : IPath
{
    public readonly Vector3Int Location;
    public readonly string TileType;
    public TileData(Vector3Int location, string tileType)
    {
        this.Location = location;
        this.TileType = tileType;
    }

    public bool CanStand()
    {
        throw new NotImplementedException();
    }

    public Vector3Int GetLocation()
    {
        return Location;
    }

    public ICollection<IPath> GetNeighbors(IMapProvider provider)
    {
        List<Vector3Int> locations = new List<Vector3Int>();
        locations.Add(Location.AddX(1));
        locations.Add(Location.AddX(-1));
        locations.Add(Location.AddZ(1));
        locations.Add(Location.AddZ(-1));

        List<IPath> neighbors = provider.GetMap().Where(i => locations.Contains(i.GetLocation())).ToList();

        //問題1 : 誰是鄰居？  我們提供 xy ， 對方 回傳 該 xy 的 tile/null
        //要在東西南北都+-1找鄰居
        //y 也要效仿
        return neighbors;
    }
}