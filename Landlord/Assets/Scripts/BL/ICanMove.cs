using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICanMove
{
    public IPath GetCurrentTile(IMapProvider provider);
    public int GetHowFarItCanBe();
    public IEnumerable<PathData> CanMoveTiles(IMapProvider mapProvider);
    public float GetPerformanceSpeed();
    public bool Seek(PathData pathData, float deltaTime, ref Vector3 newPos);
}
