using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ron.Tools;
using UnityEngine.Pool;
using UnityEngine.EventSystems;

/// <summary>
/// 主程序
/// </summary>
public class MainProcess : MonoBehaviour
{
    //static
    public static UserInfo CurrentUserInfo;


    List<IInput> inputers;
    IInput currentInputer;

    void OnWake()
    {

    }
    IMapProvider TileMap;
    void Start()
    {
        //初始化
        StaticSceneData.TileManager = GetTileMap();
        Debug.Log(StaticSceneData.TileManager == null);
        CurrentUserInfo.CurrentProcess = ProcessType.WaitForSelect;
    }

    private IMapProvider GetTileMap()
    {
        return Ron.Tools.Unity.GetComponent.GetImplement<IMapProvider>("TileMap", nameof(TileManager));
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
        switch (CurrentUserInfo.CurrentProcess)
        {
            case ProcessType.WaitForOthers:
                // 等候其他人的行動
                // 只能看不能做啥
                break;

            case ProcessType.WaitForSelect:
                // 等候選擇階段：
                // 等候 直到 取得輸入訊號
                // 將訊號轉成 scene 中的 遊戲物件，並視為 選取的物件
                // 將 選取的物件 儲存至 inputData 中
                // 從 選取的物件 取得 可以互動的選項， 並將選項顯示於螢幕上
                // 進入 等候互動階段
                var obj = GetClickObject();
                if (obj != null)
                {
                    Select(obj);
                    ChangeProcessState(ProcessType.WaitForInteract);
                }
                break;

            case ProcessType.WaitForInteract:
                // 等候互動階段：
                // 等候 直到 取得輸入訊號
                // 將訊號轉成 scene 中的 遊戲物件，並判斷此物件是否為 GUI 物件
                // 若為 GUI 物件，則 將 此回合主動的角色的移動進入CD時間，並進入 表演階段(或是進入同回合下一位角色的移動階段)

                WaitForInteract();

                break;
            case ProcessType.ActDone:

                break;
            case ProcessType.PlayAnimation:
                // 表演階段：
                // 每幀確認 互動物件的表演是否結束，
                // 若結束 取得 下一個角色， 作為主要角色
                // 回到
                break;
        }

        //////Player vs Computer(都是Inputer)
        ////取得本次移動的數據
        //var inputData = currentInputer.GetInputData();
        //if (inputData.IsFinish == false) return;

        //Move(inputData);

        ////判斷本次移動輪到誰
        //SetInputer();
        ////呈現在畫面上
    }

    private void Select(GameObject obj)
    {
        CurrentUserInfo.SelectedGameObject = obj;
        currentInputer.SetSelected(obj);
    }

    private void WaitForInteract()
    {
        // 可能要解析是取得甚麼物件
        var objType = CurrentUserInfo.SelectedGameObject.GetComponent<UiComponent>().GetObjectType();
        switch (objType)
        {
            case ObjectType.Tile:
                //ShowTileGUI
                CurrentUserInfo.CurrentCharacter.MoveTo(CurrentUserInfo.SelectedGameObject.transform.position);
                ChangeProcessState(ProcessType.ActDone);
                break;
            case ObjectType.Character:
                //ShowCharacterGUI
                break;
            case ObjectType.GuiInteract:
                //DoInteract
                //ProcessType pType = selectedGameObject.GetComponent<InteractD>().Excute(currentCharacter);
                ChangeProcessState(processType: ProcessType.PlayAnimation);
                break;
        }
    }

    private void ChangeProcessState(ProcessType processType)
    {
        CurrentUserInfo.CurrentProcess= processType;
    }

    private void Move(InputData inputData)
    {
        throw new NotImplementedException();
    }

    private GameObject GetClickObject()
    {
        // 滑鼠左鍵點擊事件(要不要替代成其他按鍵?)
        if (Input.GetMouseButtonDown(0))
        {
            // 如果是 ui 元件
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 獲取被點擊的UI元件
                GameObject clickedObject = EventSystem.current.currentSelectedGameObject;
                return clickedObject;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                return clickedObject;
            }
        }
        // 其他事件
        // ...

        // 沒有事件
        return null;
    }
}


public interface IActionResult
{
}

public interface IInput
{
    public IInteractable Selected();
    public void Cancel();
    void SetSelected(GameObject obj);
}

public class UserInfo
{
    // 目前的狀態
    public ProcessType CurrentProcess;
    public DndCharacter CurrentCharacter;
    public GameObject SelectedGameObject;
}
public enum ProcessType
{
    WaitForSelect,
    WaitForInteract,
    PlayAnimation,
    WaitForOthers,
    ActDone
}
public enum ObjectType
{
    Tile,
    Character,
    GuiInteract
}