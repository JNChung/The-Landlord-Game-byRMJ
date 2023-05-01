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
