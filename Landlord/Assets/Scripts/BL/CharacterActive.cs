using Ron.Base.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DndBuff
{
    Action<DndCharacter> Action;
    public DndBuff(Action<DndCharacter> action)
    {
        Action = action;
    }
    public void Buff(DndCharacter character)
    {
        Action(character);
    }

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

    public int DamageReduction;
}
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

    // 戰鬥屬性
    /// <summary>
    /// 力量
    /// </summary>
    public int StrengthWithBuff => ComputeBuff(nameof(DndCharacter.Strength));

    private int ComputeBuff(string v)
    {
        switch (v)
        {
            case nameof(DndCharacter.Strength):
                return this.Strength + buffs.Sum(buff => buff.Strength);
            case nameof(DndCharacter.Dexterity):
                return this.Dexterity + buffs.Sum(buff => buff.Dexterity);
            case nameof(DndCharacter.Constitution):
                return this.Constitution + buffs.Sum(buff => buff.Constitution);
            case nameof(DndCharacter.Intelligence):
                return this.Intelligence + buffs.Sum(buff => buff.Intelligence);

            default: return 0;
        }
    }

    /// <summary>
    /// 敏捷
    /// </summary>
    public int DexterityWithBuff => ComputeBuff(nameof(DndCharacter.Dexterity));
    /// <summary>
    /// 體質
    /// </summary>
    public int ConstitutionWithBuff => ComputeBuff(nameof(DndCharacter.Constitution));  //todo: 重算 CurrentHp
    /// <summary>
    /// 智力
    /// </summary>
    public int IntelligenceWithBuff => ComputeBuff(nameof(DndCharacter.Intelligence));


    public void Init(int strength, int dexterity, int constitution, int intelligence)
    {
        this.Strength = strength;
        this.Dexterity = dexterity;
        this.Constitution = constitution;
        this.Intelligence = intelligence;
    }

    //加值 (computed)
    public int BaseHp;
    public int MaxHp => BaseHp + ConstitutionBonus;
    public int CurrentHp;
    public int StrengthBonus => ComputeBonus(StrengthWithBuff) + EquipmentList.Sum(i => i.StrengthBonus);
    public int DexterityBonus => ComputeBonus(DexterityWithBuff) + EquipmentList.Sum(i => i.DexterityBonus);
    public int ConstitutionBonus => ComputeBonus(ConstitutionWithBuff) + EquipmentList.Sum(i => i.ConstitutionBonus);
    public int IntelligenceBonus => ComputeBonus(IntelligenceWithBuff) + EquipmentList.Sum(i => i.IntelligenceBonus);
    public int ArmorClass => 10 + DexterityBonus + ConstitutionBonus + EquipmentList.Sum(i => i.ArmorClass); //AC

    public int Initiative { get; internal set; }

    HashSet<Equipment> EquipmentList;
    //todo: 陳列裝備

    public List<DndBuff> buffs;

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

    public void MoveTo(Vector3 position)
    {
        throw new NotImplementedException();
    }



    #endregion
    #region ICanFight
    // todo: 下禮拜
    //命中率
    //近戰 1d20 + 力量加值 + 裝備加成 + buff加成
    //遠程 1d20 + 敏捷加值 (敵人AC)
    //法術 防禦方要骰(1d20 + 智力)  骰過傷害值 則傷害減半


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
    public DndCharacter() { }

    public void Use(string skillOrTool, Vector3Int enemyPosition)
    {
        // 檢查角色是否有
        IDndObject obj = this.Get(skillOrTool);

        if (obj != null)
        {
            obj.Effect(Position);
        }
        else
        {
            throw new InvalidCastException("物件不存在");
        }
    }

    private DndObject Get(string skillOrTool)
    {
        throw new NotImplementedException();
    }

    public void GetHurt(int damage)
    {
        this.CurrentHp -= (damage - ComputeBuff(nameof(DndBuff.DamageReduction)));
    }
}
public class Test
{

    //設計情境
    //主角(基礎值12) vs 村民A(基礎值10, 力量12)
    public void BattleTest()
    {
        #region 戰鬥初始化

        DndCharacter 主角 = new DndCharacter();
        主角.Init(12, 12, 12, 12);
        主角.AppendObject(new DndObject())

        DndCharacter 村民A = new DndCharacter();
        村民A.Init(12, 10, 10, 10);


        #endregion 戰鬥初始化

        //先攻(1d20 + 敏捷加值)
        //DndHelper.GetInitiative(主角, 村民A);//要可以客製化
        主角.Initiative = 1;
        村民A.Initiative = 2;

        //省略移動的部分
        主角.Use("餅", 村民A.Position);//角色 or 位置?

        //  8/5 todo: 
        //法術
        //完成戰鬥測試
        //整理程式碼
        //
    }
}


//規格

