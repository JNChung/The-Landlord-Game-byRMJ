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
        // 將方格的渲染器顏色更改為黃色
        squareRenderer.material.SetFloat("_OutLineWidth", 0.1f);
    }

    void OnMouseExit()
    {
        // 將方格的渲染器顏色重置為透明
        squareRenderer.material.SetFloat("_OutLineWidth", 0.0f);
    }
}
