using System;
using UnityEngine;

public abstract class DndObject
{
    public abstract void Buff(DndCharacter character);
    public abstract void Use(Vector3Int position);
}


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

//  {
//   Name:"破舊的護手",
//   StrengthBonus: 0,
//   DexterityBonus: 0,
//   ConstitutionBonus: 0,
//   IntelligenceBonus: 0,
//   ArmorClass: 2,
//  }

//xml
//< Equipment >
//<Name> 破舊的護手</Name>
//< /Equipment>

public enum WeaponType
{
    近,
    遠,
    魔法
}
public class Weapon : DndObject
{
    public DndCharacter Owner;
    public string Name;
    public string DamageRollType; //  (1d4 + 力量加值)
    public int 命中加值; //身分證 身份證
    public WeaponType WeaponType;

    public override void Buff(DndCharacter character)
    {
        throw new NotImplementedException();
    }

    public override void Use(Vector3Int position)
    {
        DndCharacter enemy = DndHelper.GetCharacter(position);
        if (Hit(enemy))
        {
            DND_Dice dice = GameManager.GerService<DND_Dice>();
            var damage = dice.Roll(this.DamageRollType) + Owner.StrengthBonus;
            enemy.GetHurt(damage);
        }
    }

    private bool Hit(DndCharacter enemy)
    {
        DND_Dice dice = GameManager.GerService<DND_Dice>();
        if (WeaponType == WeaponType.近)
        {
            var v = dice.Roll("1d20") + Owner.StrengthBonus + this.命中加值;
            return v > enemy.ArmorClass;
        }
        else if (WeaponType == WeaponType.遠)
        {
            var v = dice.Roll("1d20") + Owner.DexterityBonus + this.命中加值;
            return v > enemy.ArmorClass;
        }

        return false;
    }
}
//防具
//消耗品