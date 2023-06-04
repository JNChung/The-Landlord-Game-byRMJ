
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