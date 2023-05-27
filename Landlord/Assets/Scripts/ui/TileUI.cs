using Ron.Base.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileUI : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject canMoveUI;
    void Start()
    {
        canMoveUI = transform.Find("UiCanMove").gameObject;
        canMoveUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        canMoveUI.SetActive(Random.value > 0.3);
    }

    void GetTile()
    {
        StaticSceneData.TileManager.GetTileByCoordinate(transform.position.ToV3Int());
    }
}
