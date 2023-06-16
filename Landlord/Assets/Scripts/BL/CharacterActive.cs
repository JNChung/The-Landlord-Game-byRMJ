using Ron.Base.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DndCharacter : ICanMove//, ICanFight
{
    // 戰鬥屬性
    /// <summary>
    /// 力量
    /// </summary>
    public int Strength;
    /// <summary>
    /// 敏捷
    /// </summary>
    public int Dexterity;
    /// <summary>
    /// 體質
    /// </summary>
    public int Constitution; //todo: 重算 CurrentHp
    /// <summary>
    /// 智力
    /// </summary>
    public int Intelligence;

    //加值 (computed)
    public int BaseHp; 
    public int MaxHp => BaseHp + ConstitutionBonus; 
    public int CurrentHp;
    public int StrengthBonus => ComputeBonus(Strength) + EquipmentList.Sum(i => i.StrengthBonus);
    public int DexterityBonus => ComputeBonus(Dexterity) + EquipmentList.Sum(i => i.DexterityBonus);
    public int ConstitutionBonus => ComputeBonus(Constitution) + EquipmentList.Sum(i => i.ConstitutionBonus);
    public int IntelligenceBonus => ComputeBonus(Intelligence) + EquipmentList.Sum(i => i.IntelligenceBonus);
    public int ArmorClass => 10 + DexterityBonus + ConstitutionBonus + EquipmentList.Sum(i=>i.ArmorClass);

    HashSet<Equipment> EquipmentList;
    //todo: 陳列裝備

    /// <summary>
    /// 計算加值
    /// </summary>
    int ComputeBonus(float prop)
    {
        return ((prop - 10) / 2).ToIntTruncate();
    }

    //戰鬥行為
    #region ICanMove
    public IPath GetCurrentTile(IMapProvider mapProvider)
    {
        return mapProvider.GetTileByCoordinate(Position);
    }
    public IEnumerable<PathData> CanMoveTiles(IMapProvider mapProvider)
    {
        return moveWay(this, mapProvider);
    }
    public int GetHowFarItCanBe()
    {
        return moveAbility;
    }
    public float GetPerformanceSpeed()
    {
        return 1;
    }

    public bool Seek(PathData pathData, float deltaTime, ref Vector3 newPos)
    {
        float speed = GetPerformanceSpeed();
        return false;
    }

    internal void MoveTo(Vector3 position)
    {
        throw new NotImplementedException();
    }

    #endregion
    #region ICanFight
    // todo: 下禮拜
    #endregion

    void Upgrade()
    {
        // 決定要加強哪個屬性

    }

    // 頭銜?


    public Vector3Int Position;
    public int moveAbility;
    MoveWay moveWay;
    float deviation;//距離誤差，低於此值視為相等
    public DndCharacter(int hp, Vector3Int pos, int speed, MoveWay moveWay = null)
    {
        this.BaseHp = hp;
        this.Position = pos;
        this.moveAbility = speed;
        if (moveWay == null) moveWay = MovementAlgorithm.NormalMove;
    }
    public DndCharacter(Vector3Int pos, int speed, MoveWay moveWay = null)
    {
        this.BaseHp = DND_Dice.Roll("2d8");
        this.Position = pos;
        this.moveAbility = speed;
        if (moveWay == null) moveWay = MovementAlgorithm.NormalMove;
    }
}

//規格

