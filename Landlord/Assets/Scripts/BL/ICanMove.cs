using System.Collections.Generic;

public interface ICanMove
{
    public IPath GetCurrentTile(IMapProvider provider);
    public int GetSpeed();
    public IEnumerable<PathData> CanMoveTiles(IMapProvider mapProvider);
}
