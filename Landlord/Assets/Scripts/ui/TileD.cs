using Ron.Base.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileD : MonoBehaviour
{
    public TileData TileData { get; private set; }
    public string Type;//目前沒用到
    GameObject canMoveUI;
    void Start()
    {
        TileData = new TileData(this.transform.position.ToV3Int(), Type);
        canMoveUI = transform.Find("UiCanMove").gameObject;
        canMoveUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ShowState();
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
