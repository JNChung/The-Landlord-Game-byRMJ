using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ron.Base.Extension;
using Unity.VisualScripting;

public class Tile : IPath, IInteractable
{
    public readonly Vector3Int Location;
    public readonly string TileType;
    HashSet<string> choices = new HashSet<string>();
    public Tile(Vector3Int location, string tileType)
    {
        this.Location = location;
        this.TileType = tileType;
    }

    public bool Enabled;
    public bool Obstacle = false;

    public bool IsUiShowing;

    public void ShowCanMoveSign(bool openMoveSign = true)
    {
        IsUiShowing = openMoveSign;
    }

    public bool CanStand()
    {
        return Obstacle == false;
    }

    public Vector3Int GetLocation()
    {
        return Location;
    }

    public IEnumerable<IPath> GetNeighbors(IMapProvider provider)
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

    public IEnumerable<string> GetChoice()
    {
        choices.Add("取消");
        return choices;
    }

    public IActionResult Be(string behavior)
    {
        throw new System.NotImplementedException();
    }

    public void InitializeChoice(IEnumerable<string> choice)
    {
        choices.Clear();
        choices.AddRange(choice);
    }
}