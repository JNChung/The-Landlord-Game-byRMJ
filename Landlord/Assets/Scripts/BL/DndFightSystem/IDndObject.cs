using System;
using UnityEngine;
using UnityEngine.UI;

public class DndObjectEffectConfig
{
    // 這應該直接串連到 DndBuff身上
}
public abstract class DndObject
{
    public abstract void Buff(DndCharacter_Fight character);//指定技
    public abstract void Effect(Vector3Int position);//範圍技  todo: 我要先去地圖那邊拿 影響範圍，
} //處理附加狀態(DndCharacterAttachState)


public class Equipment
{
    public string Name;
    public string Description;
    public int StrengthBonus;
    public int DexterityBonus;
    public int ConstitutionBonus;
    public int IntelligenceBonus;
    public int ArmorClass;
}

public enum WeaponType
{
    近,
    遠,
    魔法
}

public class WeaponConfig //武器設定檔
{
    public WeaponType WeaponType;
    public int HitBonus; //身分證 身份證
    public string DamageRollType; //  (1d4 + 力量加值)，如果不骰，就留白
    public int DamageBase;
    public int StrengthWeight;
    //public int Dexterity;
    //public int Constitution;
    public int IntelligenceWeight;

    public static WeaponConfig HandOnly()
    {
        return new WeaponConfig
        {
            DamageRollType = "1d4",
            StrengthWeight = 1
        };
    }
}
public class Weapon : Equipment
{
    public static Weapon HandOnly()
    {
        return new Weapon("赤手空拳", WeaponConfig.HandOnly());
    }
    public Weapon(string name, WeaponConfig config)
    {
        _config = config;
    }
    public DndCharacter_Fight Owner;
    public string Name;
    public WeaponConfig _config;

    public int GetDamage(DndCharacter_Fight dndCharacter)
    {
        DND_Dice dice = GameManager.GerService<DND_Dice>();
        return dice.Roll(_config.DamageRollType) + dndCharacter.StrengthBonus * _config.StrengthWeight + _config.DamageBase;
    }

    private bool Hit(DndCharacter_Fight enemy)
    {
        DND_Dice dice = GameManager.GerService<DND_Dice>();
        switch (_config.WeaponType)
        {
            case WeaponType.近:
                return dice.Roll("1d20") + Owner.StrengthBonus + this._config.HitBonus > enemy.ArmorClass;
            case WeaponType.遠:
                return dice.Roll("1d20") + Owner.DexterityBonus + this._config.HitBonus > enemy.ArmorClass;
        }
        return false;
    }
}
