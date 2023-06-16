using Ron.Base.Extension;
using System.Collections.Generic;
using UnityEngine;

public static class StaticSceneData
{
    public static IMapProvider TileManager { get; set; }
    public static GameObjectConnotation GameObjectInfo { get; } = new GameObjectConnotation();
}
public class GameObjectConnotation
{
    private Dictionary<GameObject, UiComponent> datastruct = new Dictionary<GameObject, UiComponent>();
    public UiComponent Find(GameObject go)
    {
        return datastruct[go];
    }
    public void Add(GameObject go, UiComponent guide)
    {
        datastruct.Add(go, guide);
    }
}
public static class StaticValue
{

}
