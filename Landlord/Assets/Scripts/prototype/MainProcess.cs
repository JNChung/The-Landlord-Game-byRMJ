using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 主程序
/// </summary>
public class MainProcess : MonoBehaviour
{
    List<IInputer> inputers;
    private IInputer currentInputer;

    // Start is called before the first frame update
    void Start()
    {
        //初始化
    }
    private void OnEnable()
    {
        //重新初始化?
    }
    private void OnDisable()
    {
        //關閉時行為
    }
    // Update is called once per frame
    void Update()
    {
        ////Player vs Computer(都是Inputer)
        //取得本次移動的數據
        var inputData = currentInputer.GetInputData();
        if (inputData.IsFinish == false) return;

        Move(inputData);

        //判斷本次移動輪到誰
        SetInputer();
        //呈現在畫面上
    }

    private void Move(InputData inputData)
    {
        throw new NotImplementedException();
    }

    private void SetInputer()
    {
        if (currentInputer.IsEndInput())
        {
            var min = inputers.Min(i => i.GetCDValue());
            currentInputer = inputers.FirstOrDefault(i => i.GetCDValue() == min);
        }
    }

    public interface IInputer
    {
        float GetCDValue();
        InputData GetInputData();
        bool IsEndInput();
    }
}
