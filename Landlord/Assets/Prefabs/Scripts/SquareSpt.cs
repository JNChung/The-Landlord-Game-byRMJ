using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareSpt : MonoBehaviour
{
    private Renderer squareRenderer;

    void Start()
    {
        // 獲取方格的渲染器
        squareRenderer = GetComponent<Renderer>();
    }

    void OnMouseEnter()
    {
        squareRenderer.material.SetFloat("_OutLineWidth", 0.1f);
    }

    void OnMouseExit()
    {
        squareRenderer.material.SetFloat("_OutLineWidth", 0.0f);
    }
}

/*
 rouge like 的元素

背包系統?

物品

友方？

等級

戰鬥

重製 vs 保留

死亡

骰屬性

不想有太複雜的屬性戰鬥

Q:
不管甚麼角色都是共同的屬性嗎？
移動的規則
戰鬥的規則
物品系統 //會用掉
裝備系統 //用著的??
戰鬥系統
成就系統

劇情
圖片 + 文本 + 選擇

戰鬥

 */