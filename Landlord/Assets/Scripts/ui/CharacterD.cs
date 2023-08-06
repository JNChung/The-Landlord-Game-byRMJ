using Ron.Base.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterD : MonoBehaviour, UiComponent
{
    //公開變數
    public bool Selected => MainProcess.CurrentUserInfo.SelectedGameObject == this.gameObject;

    //可編輯變數
    [SerializeField] bool TestSelected;

    //私有變數
    DndCharacter_Fight character;
    Animator animator;
    Tile moveTarget;
    private bool hasMoveTarget => moveTarget != null;
    public Renderer objectRenderer; // 要改變顏色的物體的渲染器

    public ObjectType GetObjectType() => ObjectType.Character;
    private void Awake()
    {
        StaticSceneData.GameObjectInfo.Add(gameObject, this);
    }



    void Start()
    {
        InitialPosition();
        character = new DndCharacter_Fight(100, transform.position.ToV3Int(), 3);
        //animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void InitialPosition()
    {
        var newPos = transform.position.ToV3Int();
        transform.position = newPos;
    }

    void Update()
    {
        if (Selected == false)
        {
            ShowPath();
        }

        if (hasMoveTarget)
        {
            var pos = transform.position;
            //hasMoveTarget = character.Seek( /*使用者輸入的地磚*/null, Time.deltaTime, ref pos);
            transform.position = pos;
        }

        ChangeColor(Selected);
    }

    void ShowPath()
    {
        var paths = character.CanMoveTiles(StaticSceneData.TileManager);
        foreach (var path in paths)
        {
            path.Current.ShowCanMoveSign();
        }
    }

    public void ChangeColor(bool isRed)
    {
        if (isRed)
        {
            objectRenderer.material.color = Color.red;
        }
        else
        {
            objectRenderer.material.color = Color.white;

        }
    }

}
