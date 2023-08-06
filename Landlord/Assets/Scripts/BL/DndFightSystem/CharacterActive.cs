using Ron.Base.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DndFightProgram
{

}
public class Test
{

    //設計情境
    //主角(基礎值12) vs 村民A(基礎值10, 力量12)
    public void BattleTest()
    {
        #region 戰鬥初始化

        DndCharacter_Fight 主角 = new DndCharacter_Fight();
        主角.Init(strength: 12, dexterity: 12, constitution: 12, intelligence: 12);

        DndCharacter_Fight 村民A = new DndCharacter_Fight();
        村民A.Init(12, 10, 10, 10);


        #endregion 戰鬥初始化

        //省略移動的部分
        主角.Use("餅", 村民A.Position);//角色 or 位置?

        //測試目標：主角打村民A，村民A扣多少血，符合預期即可

        //  8/5 todo: 
        //法術、使用物品等戰鬥方式(附帶狀態的邏輯)
        //完成戰鬥測試
    }
}


//規格

