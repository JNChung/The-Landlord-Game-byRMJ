using Ron.Base.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DndCharacter_Fight 
{
    // 基本屬性
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
    // (受法術、裝備等)增益值
    public int StrengthWithBuff => ComputeBuff(nameof(DndCharacter_Fight.Strength));
    public int DexterityWithBuff => ComputeBuff(nameof(DndCharacter_Fight.Dexterity));
    public int ConstitutionWithBuff => ComputeBuff(nameof(DndCharacter_Fight.Constitution));  //todo: 重算 CurrentHp
    public int IntelligenceWithBuff => ComputeBuff(nameof(DndCharacter_Fight.Intelligence));
    private int ComputeBuff(string property)
    {
        double val = 0;
        switch (property)
        {
            case nameof(DndCharacter_Fight.Strength):
                val = this.Strength / 2;
                break;
            case nameof(DndCharacter_Fight.Dexterity):
                val = this.Dexterity / 2;
                break;
            case nameof(DndCharacter_Fight.Constitution):
                val = this.Constitution / 2;
                break;
            case nameof(DndCharacter_Fight.Intelligence):
                val = this.Intelligence / 2;
                break;
            default:
                break;
        }

        return (int)Math.Round(val, MidpointRounding.AwayFromZero);
    }
    public void Init(int strength, int dexterity, int constitution, int intelligence)
    {
        this.Strength = strength;
        this.Dexterity = dexterity;
        this.Constitution = constitution;
        this.Intelligence = intelligence;
    }
    public int BaseHp;
    public int MaxHp => BaseHp + ConstitutionBonus;
    public int CurrentHp;
    // 加值：用來對判定成功加成機率/成效，是由增益後屬性計算而得
    /// <summary>
    /// 力量加值
    /// </summary>
    public int StrengthBonus => ComputeBonus(StrengthWithBuff) + Weapon.StrengthBonus + EquipmentList.Sum(i => i.StrengthBonus) + BuffList.Sum(i => i.Strength);
    /// <summary>
    /// 敏捷加值
    /// </summary>
    public int DexterityBonus => ComputeBonus(DexterityWithBuff) + Weapon.DexterityBonus + EquipmentList.Sum(i => i.DexterityBonus) + BuffList.Sum(i => i.Dexterity);
    /// <summary>
    /// 體質加值
    /// </summary>
    public int ConstitutionBonus => ComputeBonus(ConstitutionWithBuff) + Weapon.ConstitutionBonus + EquipmentList.Sum(i => i.ConstitutionBonus) + BuffList.Sum(i => i.Constitution);
    /// <summary>
    /// 智力加值
    /// </summary>
    public int IntelligenceBonus => ComputeBonus(IntelligenceWithBuff) + Weapon.IntelligenceBonus + EquipmentList.Sum(i => i.IntelligenceBonus) + BuffList.Sum(i => i.Intelligence);
    public int ArmorClass => 10 + DexterityBonus + ConstitutionBonus + Weapon.ArmorClass + EquipmentList.Sum(i => i.ArmorClass) + BuffList.Sum(i => i.ArmorClass); //AC

    Weapon Weapon = Weapon.HandOnly();//暫時不考慮單手雙手的問題
    Dictionary<string, Equipment> EquipmentDic;
    List<Equipment> EquipmentList => EquipmentDic.Values.ToList(); //todo: 陳列裝備(裝備有哪些?)
    public List<DndBuff> BuffList;
    int ComputeBonus(float prop)
    {
        return ((prop - 10) / 2).ToIntTruncate();
    }
    public int Initiative { get; internal set; }// 回合制戰鬥的順序

    //裝備的設定：以身體部位來區分，一個身體部位只能一個裝備
    public void SuitUpEquipment(string bodypart, Equipment equipment)
    {
        //todo: 檢查 輸入的 bodypart 有沒有在 config
        //如果合法，就進行裝備
        EquipmentDic[bodypart] = equipment;
    }
    public void SuitUpWeapon(Weapon weapon)
    {
        Weapon = weapon;
    }

    // todo: 待討論
    //命中率
    //近戰 1d20 + 力量加值(力量 + 裝備加成 + buff加成)
    //遠程 1d20 + 敏捷加值 (敵人AC)
    //防具 & 武器的形式
    //法術 防禦方要骰(1d20 + 智力)  骰過傷害值 則傷害減半
    // 增益型法術/道具/裝備  之間的行為和區別??
    // debuff??
    //Q: 升級的機制?
    void Upgrade()
    {
        // 決定要加強哪個屬性

    }
    //有哪些要素要顯示給使用者

    // 頭銜?
    public void Use(string skillOrTool, Vector3Int enemyPosition) //動作 指定位置
    {
        // 檢查角色是否有
        DndObject obj = this.Has(skillOrTool);

        if (obj != null)
        {
            obj.Effect(Position);
        }
        else
        {
            throw new InvalidCastException("物件不存在");
        }
    }

    private DndObject Has(string skillOrTool) //todo: 確認擁有技能或道具
    {
        throw new NotImplementedException();
    }

    public void GetHurt(int damage)
    {
        this.CurrentHp -= (damage - ComputeBuff(nameof(DndBuff.DamageReduction)));
    }

    public int Attack()
    {
        return Weapon.GetDamage(this);
    }

    float deviation;//距離誤差，低於此值視為相等
    public DndCharacter_Fight()
    {

    }

}


//規格

