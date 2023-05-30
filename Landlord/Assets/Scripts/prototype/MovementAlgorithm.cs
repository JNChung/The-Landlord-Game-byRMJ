using static Ron.Base.Extension.BasicExtension;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public delegate IEnumerable<PathData> GetCanMoveTiles(ICanMove mover);
public class MovementAlgorithm
{
    public static IEnumerable<PathData> NormalMove(ICanMove mover, IMapProvider mapProvider)
    {
        List<PathData> result = new List<PathData>();
        HashSet<IPath> thisGeneration = new HashSet<IPath>();
        HashSet<IPath> pastGeneration = new HashSet<IPath>();
        //初始化
        //1. 選定初始世代
        thisGeneration.Add(mover.GetCurrentTile());
        for (int i = 1; i <= mover.GetSpeed(); i++)//不包含初始位置
        {
            //每回合要做的事
            //1. 遍歷目前世代：
            //1.1 拿到他們的次世代，倒到 可走磚
            thisGeneration = GetNextGenerationByThisGeneration(thisGeneration);// 

            result.AddRange<PathData>(thisGeneration.Select(item => new PathData(mover.GetCurrentTile(), item, i)));
        }

        return result;

        HashSet<IPath> GetNextGenerationByThisGeneration(HashSet<IPath> currentGeneration)
        {
            HashSet<IPath> nextGeneration = new HashSet<IPath>();
            foreach (var t in currentGeneration)
            {
                var newGenerations = t.GetNeighbors(mapProvider)
                    .Where(i =>
                            i.CanStand() &&
                            Not(pastGeneration.Contains(i)) &&
                            Not(currentGeneration.Contains(t)) &&
                            Not(nextGeneration.Contains(t))
                            );
                nextGeneration.AddRange(newGenerations);
                pastGeneration.Add(t);
            }
            return nextGeneration;
        }
    }
}
public interface IPath
{
    public Vector3Int GetLocation();
    public ICollection<IPath> GetNeighbors(IMapProvider provider);
    public bool CanStand();
}