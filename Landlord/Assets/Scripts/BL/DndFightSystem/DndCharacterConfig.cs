using System;
using System.Collections.Generic;

public class DndCharacterConfig //角色設定檔
{
    //可裝備部位
    public static IEnumerable<string> GetEquipmentBodyPartList()
    {
        return new string[] {
        "頭",
        "手甲",
        "胸甲",
        "腿甲",
        "鞋子",
        };
    }

    public static IEnumerable<string> GetWeaponBodyPartList()
    {
        throw new NotImplementedException();//要不要分雙手??
    }
}


//規格

