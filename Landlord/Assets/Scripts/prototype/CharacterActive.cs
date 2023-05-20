using Ron.Base.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterActive 
{
    public void ResetPosition()
    {
        //GetMap
        //FindNearestTile
        //SetPositionOnTheTile(TilePos+TileHeight)
    }
    public void CanMoveTiles()
    {
        //GetCurrentTile
        //GetTileBySpeed, Where Tile Can Be Stand
        //return tiles
        //(how to show this tiles move?)
    }
}

//規格
public interface GetTileByCoordinate
{
    //宣告
    TileData GetTileByCoordinate(Vector3Int vector3Int);//地圖編輯器"那邊"要把這個功能實作出來
}
public class TileData : HasNeighbor<TileData>   //  函數式編程 <- -> 物件導向編程   //多執行緒
{
    public Vector3Int Location;
    public string TileType;
    public TileData(GetTileByCoordinate tilemap)
    {
        this.tileMap = tilemap;
    }
    public IEnumerable<TileData> GetNeighbors()
    {
        List<TileData> neighbors = new List<TileData>();
        //問題1 : 誰是鄰居？  我們提供 xy ， 對方 回傳 該 xy 的 tile/null
        //要在東西南北都+-1找鄰居
        TileData my_nigger = tileMap.GetTileByCoordinate(this.Location.AddX(1));//使用
        if (my_nigger != null)
        {
            neighbors.Add(my_nigger);
        }
        //y 也要效仿
        return neighbors;
    }
    GetTileByCoordinate tileMap;//應該要是 地圖編輯器  現在應該是 null
    //生命週期
}//class 傳址(記憶體位址) //struct 傳值

public interface HasNeighbor<T>
{
    public IEnumerable<T> GetNeighbors();
}





public class 地圖編輯器
{
    Dictionary<Vector3Int, TileData> tileMap;//地圖，有描述鄰居關係的概念
}
public class 鄰居管理器//假設可以註冊(中介層)在目前的場景 
{
    public static IEnumerable<Tile> GetNeighbors(Tile currentTile) { return new List<Tile>(); }
}
public class Tile
{//應該會能夠取得鄰居管理器 ->
 //1. 取得當前場景，在從場景中取得鄰居管理器
    public IEnumerable<Tile> GetNeighbors()
    {
        var allNeighbors = 鄰居管理器.GetNeighbors(this);
        return allNeighbors;
    }
}

//依賴反轉  interface