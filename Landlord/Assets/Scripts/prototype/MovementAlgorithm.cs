using static Ron.Base.Extension.BasicExtension;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public delegate IEnumerable<PathData> GetCanMoveTiles(ICanMove mover);
public class MovementAlgorithm
{
    public static IEnumerable<PathData> NormalMove(ICanMove mover)
    {
        List<PathData> result = new List<PathData>();
        HashSet<TileData> thisGeneration = new HashSet<TileData>();
        HashSet<TileData> pastGeneration = new HashSet<TileData>();
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

        HashSet<TileData> GetNextGenerationByThisGeneration(HashSet<TileData> currentGeneration)
        {
            HashSet<TileData> nextGeneration = new HashSet<TileData>();
            foreach (var t in currentGeneration)
            {
                var newGenerations = t.GetNeighbors()
                    .Where(i =>
                            i.CanMove() &&
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