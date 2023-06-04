using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public delegate IEnumerable<PathData> MoveWay(ICanMove mover, IMapProvider mapProvider);
public class MovementAlgorithm//這是更核心的東西，應該跟 BL層解耦合
{
    public static IEnumerable<PathData> NormalMove(ICanMove mover, IMapProvider mapProvider)
    {
        List<PathData> result = new List<PathData>();
        HashSet<IPath> thisGeneration = new HashSet<IPath>();//這只能確定取到的都是一樣的物件，但，等值的不能判斷
        HashSet<IPath> pastGeneration = new HashSet<IPath>();
        //初始化
        //1. 選定初始世代
        thisGeneration.Add(mover.GetCurrentTile(mapProvider));
        for (int i = 1; i <= mover.GetSpeed(); i++)//必須是 mover?
        {
            //每回合要做的事
            //1. 遍歷目前世代：
            //1.1 拿到他們的次世代，倒到 可走磚
            thisGeneration = GetNextGenerationByThisGeneration(thisGeneration);// 

            result.AddRange<PathData>(thisGeneration.Select(item => new PathData(mover.GetCurrentTile(mapProvider), item, i)));
        }

        return result;

        HashSet<IPath> GetNextGenerationByThisGeneration(HashSet<IPath> currentGeneration)
        {
            HashSet<IPath> nextGeneration = new HashSet<IPath>();
            foreach (var t in currentGeneration)
            {
                var newGenerations = t.GetNeighbors(mapProvider);//這段好像有點問題，先不用 linq
                foreach (var i in newGenerations)
                {
                    if (i.CanStand() == false)
                        continue;
                    if (pastGeneration.Contains(i))
                        continue;
                    if (currentGeneration.Contains(i))
                        continue;
                    if (nextGeneration.Contains(i))
                        continue;
                    nextGeneration.Add(i);
                }
                pastGeneration.Add(t);
            }
            return nextGeneration;
        }
    }
}
