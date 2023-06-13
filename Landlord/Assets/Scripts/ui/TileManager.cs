using Ron.Base.Extension;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour, IMapProvider
{
    static List<GameObject> layerObjs = new List<GameObject>();
    static Dictionary<GameObject, Dictionary<Vector3Int, GameObject>> k_Layer_v_TileDictionary = new Dictionary<GameObject, Dictionary<Vector3Int, GameObject>>();

    static GameObject tileMap;
    IPath IMapProvider.GetTileByCoordinate(Vector3Int vector3Int)
    {
        RebuildTileMap();
        GameObject layer = null;
        while (layer == null)
        {
            Debug.Log($"尋找layer:{vector3Int.y}");
            layer = layerObjs.FirstOrDefault(i => i.transform.position.y == vector3Int.y);
            vector3Int.y--;
        }
        Debug.Log("找到layer");

        var result = k_Layer_v_TileDictionary[layer][vector3Int.WithY()].GetComponent<TileD>().TileData;
        return result;
    }
    ICollection<IPath> IMapProvider.GetMap()
    {
        HashSet<IPath> paths = new HashSet<IPath>();
        foreach (var layer in layerObjs)
        {
            foreach (var tile in k_Layer_v_TileDictionary[layer].Values)
            {
                var tileComp = tile.GetComponent<TileD>();
                paths.Add(tileComp.TileData);
            }
        }

        return paths;
    }

    private void RebuildTileMap()
    {
        if (tileMap != null) return;
        Debug.LogWarning("tileMap物件消失，嘗試重建中");
        CleanLayerContainer();
        tileMap = GameObject.Find("TileMap");
        if (tileMap != null)
        {
            Debug.LogWarning("偵測到既存 tileMap，重建圖層");
            Transform[] trs = tileMap.GetComponentsInChildren<Transform>();
            foreach (var item in trs)
            {
                if (item.parent == tileMap.transform)//第一層子物件
                {
                    AddNewLayer(item.gameObject);
                }
            }
        }
        else
        {
            Debug.LogWarning("找不到 tileMap，重建主物件");
            tileMap = new GameObject();
            tileMap.name = "TileMap";
            AddDefaultLayer();
        }
        RebuildMapDicByHierachy();
    }
    private void AddDefaultLayer()
    {
        GameObject go = new GameObject();
        go.name = "Default";
        go.transform.SetParent(tileMap.transform);
        AddNewLayer(go);
    }
    private void AddNewLayer(GameObject go)
    {
        layerObjs.Add(go);
        k_Layer_v_TileDictionary.Add(go, new Dictionary<Vector3Int, GameObject>());//建立圖層時，要順便建立該圖層的字典容器。
    }
    private void CleanLayerContainer()
    {
        layerObjs.Clear();
        k_Layer_v_TileDictionary.Clear();
    }
    void RebuildMapDicByHierachy()
    {
        Debug.LogWarning("重建字典...");
        if (tileMap == null)
        {
            Debug.LogWarning("重建字典失敗，物件不存在");
            return;
        }

        for (int i = 0; i < layerObjs.Count(); i++)
        {
            k_Layer_v_TileDictionary[layerObjs[i]].Clear();
            Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();
            foreach (var item in children)
            {
                if (item.parent == layerObjs[i].transform)
                {
                    Vector3Int posInt = item.transform.position.ToV3Int();
                    Vector3Int dicKey = posInt.WithY();
                    item.transform.position = posInt;//對齊方塊
                    if (!k_Layer_v_TileDictionary[layerObjs[i]].ContainsKey(dicKey))
                    {
                        k_Layer_v_TileDictionary[layerObjs[i]].Add(dicKey, item.gameObject);
                    }
                }
            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
