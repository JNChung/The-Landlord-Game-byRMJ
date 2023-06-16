using Ron.Base.Extension;
using Ron.Tools.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileD : MonoBehaviour, UiComponent
{
    public bool Selected => MainProcess.CurrentUserInfo.SelectedGameObject == this.gameObject;

    //可編輯變數
    [SerializeField] bool TestSelected;
    [SerializeField] List<string> Choice;
    public Tile TileData { get; private set; }
    public string Type;//目前沒用到
    GameObject canMoveUI;


    public ObjectType GetObjectType() => ObjectType.Tile;
    private void Awake()
    {
        StaticSceneData.GameObjectInfo.Add(gameObject, this);
    }

    void Start()
    {
        TileData = new Tile(this.transform.position.ToV3Int(), Type);
        TileData.InitializeChoice(Choice);

        canMoveUI = transform.FindOffspring("UiCanMove").gameObject;
        canMoveUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShowState();
        //ShowSelected();
    }


    private void ShowState()
    {
        canMoveUI.SetActive(TileData.IsUiShowing);
    }





    // Start is called before the first frame update


    // Update test
    //void Update()
    //{
    //    canMoveUI.SetActive(Random.value > 0.3);
    //}

    //void GetTile()
    //{
    //    StaticSceneData.TileManager.GetTileByCoordinate(transform.position.ToV3Int());
    //}
}
