using System.Collections.Generic;

public class DndBuff
{
    public int KeepRound;//持續時間(int.MaxValue 為最大值)
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
    public int Constitution;
    /// <summary>
    /// 智力
    /// </summary>
    public int Intelligence;
    public int ArmorClass;
    public int Damage;

    public int DamageReduction;

    //對角色以外的物件呢？
    //要去思考的點是：附加狀態，是因為作用於該物體，還是 使用的物品本身的性質滯留於物件表面導致。
    //ex: 潑灑酒精，應該是所有物體都會被點燃，是因為燃燒的本身是酒精。
}

public class DndCharacterAttachState //給角色的附加狀態  在不改變基礎數值的情況下，針對比例去減少/增加角色的能力
{
    //public int KeepRound;//持續時間(int.MaxValue 為最大值)
    public string StateName;
    public int Damage;
    public int FearWeight;//恐懼
    public int BlindWeight;//致盲
    public List<string> ExtensionSubstance;
    //感染?燒灼?冰凍?......
}


//規格

