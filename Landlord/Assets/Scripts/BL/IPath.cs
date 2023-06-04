using System.Collections.Generic;
using UnityEngine;

public interface IPath
{
    public Vector3Int GetLocation();
    public IEnumerable<IPath> GetNeighbors(IMapProvider provider);
    public bool CanStand();

    public void ShowPath(bool open = true);
}