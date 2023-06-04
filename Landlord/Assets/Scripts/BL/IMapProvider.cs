using System.Collections.Generic;
using UnityEngine;

public interface IMapProvider
{
    IPath GetTileByCoordinate(Vector3Int vector3Int);//地圖編輯器"那邊"要把這個功能實作出來
    ICollection<IPath> GetMap();
}