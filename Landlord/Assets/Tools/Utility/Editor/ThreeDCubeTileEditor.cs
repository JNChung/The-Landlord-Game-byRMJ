using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using RonTools;
using Ron.Base.Extension;
using Codice.Utils;
using UnityEngine.Tilemaps;
using System.Collections.ObjectModel;
using System.Linq;
using static UnityEditor.PlayerSettings;
using UnityEditor.SceneManagement;

namespace Ron.Tools
{
    public class ThreeDCubeTileEditor : EditorWindow
    {
        public static ThreeDCubeTileEditor tileEditor;
        public ThreeDCubeTileEditor()
        {
            tileEditor = this;
        }

        UserOperateInfo userOperateInfo = new UserOperateInfo();
        static bool onPainting = false;
        void OnSceneGUI(SceneView sceneView)//繪製各種UI物件
        {
            #region 註解掉的
            //Handles.BeginGUI();
            //{
            //   若有需要在 SceneView 繪製各種UI功能
            //}
            //Handles.EndGUI();
            //onPainting = true;  //測試用
            #endregion

            if (onPainting == false) return;

            UserOperateInfo.SetMouseDown(out int controlID, out Event e);

            Vector3 pos;
            Vector3Int posInt = Vector3Int.zero;
            bool mouseInWorld = MouseToWorldPosition(e.mousePosition, Mathf.RoundToInt(layerObjs[UserOperateInfo.selectedLayer].transform.position.y), out pos);

            if (mouseInWorld)
            {
                posInt = V3ToV3Int(pos);
                PaintGridCursor(posInt);
            }
            if (UserOperateInfo.mouseDown && mouseInWorld)
            {
                Vector3Int key = posInt.WithY();
                if (e.shift)//刪除方塊
                {
                    DeleteTile(key);
                }
                else//繪製方塊
                {
                    if (UserOperateInfo.autoClearOverlapCube)
                    {
                        foreach (var dic in k_Layer_v_Tile.Values)
                        {
                            DeleteTile(key, dic);
                        }
                    }

                    if (k_Layer_v_Tile[layerObjs[UserOperateInfo.selectedLayer]].ContainsKey(key) && UserOperateInfo.replaceItem)
                    {
                        DeleteTile(key);
                    }

                    //繪製方塊
                    GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(mapItemPrefabs[UserOperateInfo.selectedMapItem]);
                    cube.transform.position = posInt;
                    cube.transform.SetParent(layerObjs[UserOperateInfo.selectedLayer].transform);
                    k_Layer_v_Tile[layerObjs[UserOperateInfo.selectedLayer]].Add(key, cube);
                }
            }

        }
        #region OnSceneGUI 專用
        void DeleteTile(Vector3Int key)
        {
            GameObject goToBeDeleted;
            if (k_Layer_v_Tile[layerObjs[UserOperateInfo.selectedLayer]].TryGetValue(key, out goToBeDeleted))
            {
                k_Layer_v_Tile[layerObjs[UserOperateInfo.selectedLayer]].Remove(key);
                DestroyImmediate(goToBeDeleted);
            }
        }
        void DeleteTile(Vector3Int key, Dictionary<Vector3Int, GameObject> dic)
        {
            GameObject goToBeDeleted;
            if (dic.TryGetValue(key, out goToBeDeleted))
            {
                dic.Remove(key);
                DestroyImmediate(goToBeDeleted);
            }
        }

        private void PaintGridCursor(Vector3Int posInt)
        {
            posInt = new Vector3Int((posInt.x / mapUnitSize) * mapUnitSize, posInt.y, (posInt.z / mapUnitSize) * mapUnitSize);//貼齊格線小技巧

            float cursorOffset = (float)mapUnitSize / 2f;
            Vector3 p1 = new Vector3(posInt.x - cursorOffset, posInt.y - 0.5f, posInt.z - cursorOffset);
            Vector3 p2 = new Vector3(posInt.x - cursorOffset, posInt.y - 0.5f, posInt.z + cursorOffset);
            Vector3 p3 = new Vector3(posInt.x + cursorOffset, posInt.y - 0.5f, posInt.z + cursorOffset);
            Vector3 p4 = new Vector3(posInt.x + cursorOffset, posInt.y - 0.5f, posInt.z - cursorOffset);

            Color handlesColor = Handles.color;
            int thickness = 2;
            Handles.color = cursorColor;
            Handles.DrawLine(p1, p2, thickness);
            Handles.DrawLine(p2, p3, thickness);
            Handles.DrawLine(p3, p4, thickness);
            Handles.DrawLine(p4, p1, thickness);
            SceneView.RepaintAll();
            Handles.color = handlesColor;
        }

        bool MouseToWorldPosition(Vector3 mousePosition, int layerHeight, out Vector3 MouseWorldPosition)
        {
            Vector3Int h = new Vector3Int(0, layerHeight, 0);
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
            if (mouseRay.direction.y <= 0) //代表滑鼠點擊處在畫面內
            {
                mouseRay.origin -= h;
                float t = -mouseRay.origin.y / mouseRay.direction.y;
                MouseWorldPosition = mouseRay.origin + t * mouseRay.direction + h;
                return true;
            }
            else
            {
                MouseWorldPosition = Vector3.zero;
                return false;
            }
        }

        #endregion

        static GameObject tileMap;
        GameObject TileMap
        {
            set { tileMap = value; }
            get
            {
                RebuildTileMap();
                return tileMap;
            }
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
        }
        private void AddDefaultLayer()
        {
            GameObject go = new GameObject();
            go.name = "Default";
            go.transform.SetParent(TileMap.transform);
            AddNewLayer(go);
            UserOperateInfo.selectedLayer = 0;
        }
        private void AddNewLayer(GameObject go)
        {
            layerObjs.Add(go);
            layerNames.Add(go.name);
            k_Layer_v_Tile.Add(go, new Dictionary<Vector3Int, GameObject>());//建立圖層時，要順便建立該圖層的字典容器。
        }
        private void CleanLayerContainer()
        {
            layerObjs.Clear();
            layerNames.Clear();
            k_Layer_v_Tile.Clear();
        }
        //containers
        static List<GameObject> mapItemPrefabs = new List<GameObject>();
        static List<string> mapItemNames = new List<string>();
        static List<Texture> mapItemIcons = new List<Texture>();

        static List<GameObject> layerObjs = new List<GameObject>();
        static List<string> layerNames = new List<string>();
        static Dictionary<GameObject, Dictionary<Vector3Int, GameObject>> k_Layer_v_Tile = new Dictionary<GameObject, Dictionary<Vector3Int, GameObject>>();

        //使用者操作狀態



        private void OnFocus()
        {
            RebuildMapItemDataByIO();
            RebuildTileMap();
            RebuildMapDicByHierachy();
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += this.OnSceneGUI;

            OnFocus();
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }
        void RebuildMapDicByHierachy()
        {
            Debug.LogWarning("重建字典...");
            if (tileMap == null)
            {
                Debug.LogWarning("重建字典失敗，物件不存在");
                return;
            }

            for (int i = 0; i < layerNames.Count(); i++)
            {
                k_Layer_v_Tile[layerObjs[i]].Clear();
                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();
                foreach (var item in children)
                {
                    if (item.parent == layerObjs[i].transform)
                    {
                        Vector3Int posInt = V3ToV3Int(item.transform.position);
                        Vector3Int dicKey = posInt.WithY();
                        item.transform.position = posInt;//對齊方塊
                        if (!k_Layer_v_Tile[layerObjs[i]].ContainsKey(dicKey))
                        {
                            k_Layer_v_Tile[layerObjs[i]].Add(dicKey, item.gameObject);
                        }
                    }
                }
            }
        }

        #region interface functions

        #endregion

        #region IO Functions
        string filepath = "Tools/Utility/Editor/Datas/MapItems.json";
        /// <summary>
        /// 存檔用資料
        /// </summary>
        class MapItem
        {
            public string[] names;
            public MapItem(string[] names)
            {
                this.names = names;
            }
        }
        void SaveMapItemData()
        {
            SaveAndLoadForIO saveAndLoad = new SaveAndLoadForIO(filepath);
            saveAndLoad.SaveJson(new MapItem(mapItemNames.ToArray()));
        }
        void RebuildMapItemDataByIO()
        {
            mapItemPrefabs.Clear();
            mapItemNames.Clear();
            mapItemIcons.Clear();
            UserOperateInfo.selectedMapItem = 0;

            SaveAndLoadForIO saveAndLoad = new SaveAndLoadForIO(filepath);
            if (!saveAndLoad.FileExists()) return;
            MapItem data = saveAndLoad.LoadJson<MapItem>();
            foreach (string name in data.names)
            {
                AddMapItemToContainer(name);
            }

            Repaint();//全部重繪，以避免有時候縮圖顯示不出來的問題
        }

        void SaveMap(string filename)
        {
            TileMapData map = new TileMapData();
            RebuildMapItemDataByIO();
            RebuildTileMap();
            RebuildMapDicByHierachy();

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                map.camPos = cam.position;
                map.camRot = cam.eulerAngles;
                map.camFDV = cam.GetComponent<Camera>().fieldOfView;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                map.playerPos = V3ToV3Int(player.transform.position);
                map.playerRot = player.transform.eulerAngles;
            }

            map.resBlocks = mapItemNames.ToArray();
            map.layerDatas = new TileMapLayer[layerObjs.Count];

            for (int i = 0; i < layerObjs.Count; i++)
            {
                map.layerDatas[i].name = layerObjs[i].name;
                map.layerDatas[i].height = Mathf.RoundToInt(layerObjs[i].transform.position.y);
                map.layerDatas[i].blocks = new List<Block>();

                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();

                foreach (var item in children)
                {
                    if (item.parent == layerObjs[i].transform)
                    {
                        //取得地板原始的Prefab
                        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(item.gameObject);
                        //取得Prefab在硬碟中存放的位置
                        string assetPath = AssetDatabase.GetAssetPath(prefab);
                        //找出相同的檔名即為該編號
                        int index = 0;
                        for (int j = 0; j < map.resBlocks.Length; j++)
                        {
                            if (assetPath.ToLower() == map.resBlocks[j].ToLower())
                            {
                                index = j;
                                break;
                            }

                        }

                        map.layerDatas[i].blocks.Add(new Block(index, V3ToV3Int(item.position)));
                    }
                }

            }

            SaveAndLoadForIO saveAndLoad = new SaveAndLoadForIO(filename);
            saveAndLoad.SaveJson(map);
        }
        void LoadMap(string filename)
        {
            TileMapData map = new TileMapData();
            SaveAndLoadForIO saveAndLoad = new SaveAndLoadForIO(filename);
            map = saveAndLoad.LoadJson<TileMapData>();

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                cam.position = map.camPos;
                cam.eulerAngles = map.camRot;
                Camera.main.GetComponent<Camera>().fieldOfView = map.camFDV;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = map.playerPos;
                player.transform.eulerAngles = map.playerRot;
            }

            //清空資源列表
            mapItemPrefabs.Clear();
            mapItemNames.Clear();
            mapItemIcons.Clear();
            UserOperateInfo.selectedMapItem = 0;
            CleanLayerContainer();
            DestroyImmediate(tileMap);

            //讀取資源列表
            foreach (var item in map.resBlocks)
            {
                var go = (GameObject)AssetDatabase.LoadAssetAtPath(item, typeof(GameObject));

                mapItemPrefabs.Add(go);
                mapItemNames.Add(item);
                mapItemIcons.Add(AssetPreview.GetAssetPreview(go));
            }

            SaveMapItemData();

            tileMap = new GameObject();
            tileMap.name = "TileMap";
            int dicNum = 0;
            foreach (var item in map.layerDatas)
            {
                GameObject go = new GameObject();
                go.name = item.name;
                go.transform.position = new Vector3(0, item.height, 0);
                go.transform.SetParent(TileMap.transform);
                AddNewLayer(go);
                foreach (var b in item.blocks)
                {
                    GameObject ob = (GameObject)PrefabUtility.InstantiatePrefab(mapItemPrefabs[b.index]);
                    ob.transform.position = b.pos;
                    ob.transform.SetParent(go.transform);
                    var dicPos = new Vector3Int(b.pos.x, 0, b.pos.z);
                    k_Layer_v_Tile[layerObjs[dicNum]].Add(dicPos, ob);
                }
                dicNum++;
            }

        }
        #endregion

        #region User Interface
        Vector2 scrollPos;
        float contentWidth;
        private string tempPath = "";
        static float iconSize = 80;//縮圖尺寸
        static string tempFilename = "";

        /// <summary>
        /// Edit Window
        /// </summary>
        private void OnGUI()
        {
            //初始化、格式化
            ChineseSupport();
            //Texture2D result = new Texture2D(1, 1);//加入這一行才能即時顯示格線。
            UpdateContentWidth();
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);//box要用的風格檔。
            boxStyle.normal.textColor = Color.white;
            boxStyle.fixedWidth = contentWidth;

            //draw ui component
            scrollPos = Nested._(() => EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(500)), () =>
            {
                GUILayout.Box("地圖元件", boxStyle);
                ShowMapComponentManageUI();
                DisplayMapComponents();
                MakeGap();

                GUILayout.Box("圖層", boxStyle);
                ShowLayerManageUI();
                MakeGap();

                GUILayout.Box("地圖製作", boxStyle);
                ShowOperateUI();
                MakeGap();

                GUILayout.Box("檔案", boxStyle);
                ShowIOManageUI();

            }, () => EditorGUILayout.EndScrollView());
            #region functions
            void ShowMapComponentManageUI()
            {
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (GUILayout.Button("加入地圖元件")) { AddMapItem(); }
                        if (GUILayout.Button("刪除地圖元件")) { RemoveMapItem(); }
                        if (GUILayout.Button("刪除全圖元件")) { RemoveAllMapItem(); }
                        //顯示尺寸用的滑桿
                        iconSize = GUILayout.HorizontalSlider(iconSize, 40, 120, GUILayout.Width(60));
                    }, () => GUILayout.EndHorizontal());
                }, () => GUILayout.EndVertical());
            }
            void ShowLayerManageUI()
            {
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (!UserOperateInfo.newLayer && !UserOperateInfo.editLayer)//彈窗效果：不是新增和編輯圖層就不顯示按鈕
                        {
                            if (GUILayout.Button("新增圖層"))
                            {
                                AddMapLayer();
                            }
                            if (GUILayout.Button("編輯圖層"))
                            {
                                ModifyMapLayer();
                            }
                            if (GUILayout.Button("刪除圖層"))
                            {
                                RemoveMapLayer();
                            }
                        }
                        if (UserOperateInfo.newLayer)
                        {
                            AddMapLayerImplement();
                        }
                        else if (UserOperateInfo.editLayer)//editLayer
                        {
                            ModifyMapLayerImplement();
                        }
                    }, () => GUILayout.EndHorizontal());
                    DisplayLayerList();
                }, () => GUILayout.EndVertical());
            }
            void ShowOperateUI()
            {
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    //可實作設定單位尺寸
                    //可實作設定格線尺寸
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        UserOperateInfo.autoClearOverlapCube = GUILayout.Toggle(UserOperateInfo.autoClearOverlapCube, "自動清除不同圖層重疊方塊");
                        UserOperateInfo.replaceItem = GUILayout.Toggle(UserOperateInfo.replaceItem, "自動取代");
                    }, () => GUILayout.EndHorizontal());
                    if (onPainting)
                    {
                        ShowActivatedOnDrawButton();
                    }
                    else
                    {
                        ShowStartPaintOnDrawButton();
                    }
                }, () => GUILayout.EndVertical());
            }
            void ShowActivatedOnDrawButton()
            {
                GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
                btnStyle.normal.textColor = Color.yellow;
                btnStyle.normal.background = MakeTex(10, 10, Color.blue);
                if (GUILayout.Button("繪圖中...", btnStyle))
                {
                    onPainting = false;
                }

                Texture2D MakeTex(int width, int height, Color color)
                {
                    Color[] pixels = new Color[width * height];
                    int length = pixels.Length;
                    for (int i = 0; i < length; i++)
                    {
                        pixels[i] = color;
                    }
                    Texture2D r = new Texture2D(width, height);
                    r.SetPixels(pixels, 0);
                    r.Apply();
                    return r;
                }
            }
            void ShowStartPaintOnDrawButton()
            {
                GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
                btnStyle.normal.textColor = Color.white;
                btnStyle.normal.background = GUI.skin.button.normal.background;
                if (GUILayout.Button("開始繪圖", btnStyle))
                {
                    onPainting = true;
                }
            }
            void ShowIOManageUI()
            {
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (GUILayout.Button("新地圖")) { CreateNewMap(); }
                        if (GUILayout.Button("儲存檔案")) { SaveMapData(); }
                        if (GUILayout.Button("載入舊檔")) { LoadMapData(); }
                    }, () => GUILayout.EndHorizontal());
                }, () => GUILayout.EndVertical());
            }
            void MakeGap()
            {
                GUILayout.Space(20);
            }
            void ChineseSupport()
            {
                Input.imeCompositionMode = IMECompositionMode.On;//讓輸入單元支援中文。
            }
            void AddMapItem()
            {
                string defaultPath = TryGetPastDirectorPath();
                if (string.IsNullOrEmpty(defaultPath)) defaultPath = Application.dataPath;

                string prefabFull = EditorUtility.OpenFilePanel("選取地圖元件", defaultPath, "prefab,fbx");
                if (prefabFull == "") return;//使用者按下取消

                tempPath = Path.GetDirectoryName(prefabFull);
                string prefabShort = "Assets" + prefabFull.Substring(Application.dataPath.Length);

                AddMapItemToContainer(prefabShort);
                SaveMapItemData();
            }
            void RemoveMapItem()
            {
                if (mapItemPrefabs.Count > 0)
                {
                    if (!EditorUtility.DisplayDialog("刪除地圖元件", $"確定要移除元件「{mapItemNames[UserOperateInfo.selectedMapItem]}」嗎？", "確定", "取消")) return;

                    RemoveMapItemFromContainer();
                    SaveMapItemData();
                }

                void RemoveMapItemFromContainer()
                {
                    mapItemPrefabs.Remove(mapItemPrefabs[UserOperateInfo.selectedMapItem]);
                    mapItemNames.Remove(mapItemNames[UserOperateInfo.selectedMapItem]);
                    mapItemIcons.Remove(mapItemIcons[UserOperateInfo.selectedMapItem]);
                }
            }
            void RemoveAllMapItem()
            {
                if (mapItemPrefabs.Count > 0)
                {
                    if (!EditorUtility.DisplayDialog("刪除全部元件", $"確定要移除全部元件嗎？", "確定", "取消")) return;

                    mapItemPrefabs.Clear();
                    mapItemIcons.Clear();
                    mapItemNames.Clear();
                    UserOperateInfo.selectedMapItem = 0;

                    SaveMapItemData();
                }

            }
            void DisplayMapComponents()
            {
                if (mapItemNames.Count > 0)
                {
                    if (iconSize > 40.1f)//顯示縮圖
                    {
                        int xCount = (int)((contentWidth - 20) / iconSize);
                        int lines = ((mapItemNames.Count - 1) / xCount) + 1;
                        UserOperateInfo.selectedMapItem = GUILayout.SelectionGrid(UserOperateInfo.selectedMapItem, mapItemIcons.ToArray(), xCount, GUILayout.Width(contentWidth - 20), GUILayout.Height(lines * iconSize));
                    }
                    else //顯示詳細文字列表
                    {
                        UserOperateInfo.selectedMapItem = GUILayout.SelectionGrid(UserOperateInfo.selectedMapItem, mapItemNames.ToArray(), 1, GUILayout.Width(contentWidth - 20));
                    }
                }
            }
            string TryGetPastDirectorPath()
            {
                return tempPath;
            }
            void UpdateContentWidth()
            {
                contentWidth = position.width - 20;
            }
            void AddMapLayer()
            {
                UserOperateInfo.newLayer = true;
                UserOperateInfo.currentLayerName = "New Layer";
                UserOperateInfo.currentLayerHeight = 0;
            }
            void AddMapLayerImplement()
            {
                Nested._(() => GUILayout.BeginVertical(), () =>
                {
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        GUILayout.Label("圖層名稱", GUILayout.Width(60));
                        UserOperateInfo.currentLayerName = EditorGUILayout.TextField(UserOperateInfo.currentLayerName);
                    }, () => GUILayout.EndHorizontal());

                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        GUILayout.Label("圖層高度", GUILayout.Width(60));
                        UserOperateInfo.currentLayerHeight = EditorGUILayout.IntField(UserOperateInfo.currentLayerHeight);
                        if (GUILayout.Button("-")) UserOperateInfo.currentLayerHeight -= 1;
                        if (GUILayout.Button("+")) UserOperateInfo.currentLayerHeight += 1;
                        if (GUILayout.Button("確定"))
                        {
                            GameObject go = new GameObject();
                            go.name = UserOperateInfo.currentLayerName;
                            go.transform.position = new Vector3(0, UserOperateInfo.currentLayerHeight, 0);
                            go.transform.SetParent(TileMap.transform);
                            AddNewLayer(go);
                            UserOperateInfo.newLayer = false;
                            UserOperateInfo.currentLayerName = "";
                        }
                        if (GUILayout.Button("取消"))
                        {
                            UserOperateInfo.newLayer = false;
                            UserOperateInfo.currentLayerName = "";
                        }

                    }, () => GUILayout.EndHorizontal());


                }, () => GUILayout.EndVertical());
            }
            void ModifyMapLayer()
            {
                UserOperateInfo.editLayer = true;
                UserOperateInfo.currentLayerName = layerNames[UserOperateInfo.selectedLayer];
                UserOperateInfo.currentLayerHeight = Mathf.RoundToInt(layerObjs[UserOperateInfo.selectedLayer].transform.position.y);
            }
            void ModifyMapLayerImplement()
            {
                Nested._(() => GUILayout.BeginVertical(), () =>
                {
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        GUILayout.Label("圖層名稱", GUILayout.Width(60));
                        UserOperateInfo.currentLayerName = EditorGUILayout.TextField(UserOperateInfo.currentLayerName);
                    }, () => GUILayout.EndHorizontal());

                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        GUILayout.Label("圖層高度", GUILayout.Width(60));
                        UserOperateInfo.currentLayerHeight = EditorGUILayout.IntField(UserOperateInfo.currentLayerHeight);
                        if (GUILayout.Button("-")) UserOperateInfo.currentLayerHeight -= 1;
                        if (GUILayout.Button("+")) UserOperateInfo.currentLayerHeight += 1;
                        if (GUILayout.Button("確定"))
                        {
                            layerObjs[UserOperateInfo.selectedLayer].name = UserOperateInfo.currentLayerName;
                            layerObjs[UserOperateInfo.selectedLayer].transform.position = new Vector3(0, UserOperateInfo.currentLayerHeight, 0);
                            layerNames[UserOperateInfo.selectedLayer] = UserOperateInfo.currentLayerName;
                            UserOperateInfo.editLayer = false;
                            UserOperateInfo.currentLayerName = "";
                        }
                        if (GUILayout.Button("取消"))
                        {
                            UserOperateInfo.editLayer = false;
                            UserOperateInfo.currentLayerName = "";
                        }
                    }, () => GUILayout.EndHorizontal());


                }, () => GUILayout.EndVertical());

            }
            void DisplayLayerList()
            {
                UserOperateInfo.selectedLayer = GUILayout.SelectionGrid(UserOperateInfo.selectedLayer, layerNames.ToArray(), 1, GUILayout.Width(contentWidth));
            }
            void RemoveMapLayer()
            {
                if (!EditorUtility.DisplayDialog("刪除圖層", $"確定要刪除圖層「{layerNames[UserOperateInfo.selectedLayer]}」嗎？\n該圖層的資料會全部刪除", "確定", "取消")) return;
                DestroyImmediate(layerObjs[UserOperateInfo.selectedLayer]);//刪除圖層管理的GameObject
                layerObjs.Remove(layerObjs[UserOperateInfo.selectedLayer]);
                layerNames.Remove(layerNames[UserOperateInfo.selectedLayer]);
                k_Layer_v_Tile.Remove(layerObjs[UserOperateInfo.selectedLayer]);
                UserOperateInfo.selectedLayer = 0;
            }
            void CreateNewMap()
            {
                if (!EditorUtility.DisplayDialog("新地圖", "現有地圖資料會被清空，確定要開新地圖嗎？", "確定", "取消"))
                {
                    return;
                }

                CleanLayerContainer();
                DestroyImmediate(tileMap);
                var t = TileMap;
                tempFilename = "";

            }
            void SaveMapData()
            {
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                string defaultPath = tempPath;
                if (defaultPath == "") defaultPath = Application.dataPath;

                string filename = EditorUtility.SaveFilePanel("儲存檔案", defaultPath, tempFilename, "json");

                if (filename != "")
                {
                    tempPath = Path.GetDirectoryName(filename);
                    tempFilename = Path.GetFileName(filename);
                    SaveMap(filename);
                }
            }
            void LoadMapData()
            {
                UserOperateInfo.selectedMapItem = 0;
                UserOperateInfo.selectedLayer = 0;
                string defaultPath = tempPath;
                if (defaultPath == "") defaultPath = Application.dataPath;

                string filename = EditorUtility.OpenFilePanel("載入舊檔", defaultPath, "json");
                if (filename != "")
                {
                    tempPath = Path.GetDirectoryName(filename);
                    tempFilename = Path.GetFileName(filename);
                    LoadMap(filename);
                    EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                }
            }
            #endregion
        }
        #endregion

        #region MapItem Container Operations

        private void AddMapItemToContainer(string name)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
            mapItemPrefabs.Add(go);
            mapItemNames.Add(name);
            mapItemIcons.Add(AssetPreview.GetAssetPreview(go));
        }
        #endregion

        static Vector3Int V3ToV3Int(Vector3 val)
        {
            int x = Mathf.RoundToInt(val.x);
            int y = Mathf.RoundToInt(val.y);
            int z = Mathf.RoundToInt(val.z);
            return new Vector3Int(x, y, z);
        }

        // field for RenderGridGizmo
        static bool showGrid = true;
        static int gridCount = 19;//格線數量，太多會影響效能
        static Color cursorColor = Color.yellow;
        static Color gridColor = Color.gray;
        static int mapUnitSize = 1;
        /// <summary>
        /// 畫些東西到 Scene View 上
        /// </summary>
        [DrawGizmo(GizmoType.NonSelected)]
        static void RenderGridGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (layerObjs.Count == 0 || tileMap == null) return;
            if (showGrid)
            {
                int s = (gridCount + 1) / 2;
                int h = Mathf.RoundToInt(layerObjs[UserOperateInfo.selectedLayer].transform.position.y);

                Transform cam = SceneView.currentDrawingSceneView.camera.transform;
                Ray cf = new Ray(cam.position, cam.forward);
                Plane plane = new Plane(Vector3.up, new Vector3(0, h, 0));
                if (!plane.Raycast(cf, out float enter)) return;
                Vector3Int center = V3ToV3Int(cf.GetPoint(enter));
                Gizmos.color = gridColor;
                float offset = 0.5f * mapUnitSize;
                for (int i = -s; i < s; i++)
                {
                    Gizmos.DrawLine(center + new Vector3(-s * mapUnitSize + offset, -1f, i * mapUnitSize + offset), center +
                        new Vector3(s * mapUnitSize - offset, -1f, i * mapUnitSize + offset));
                    Gizmos.DrawLine(center + new Vector3(i * mapUnitSize + offset, -1f, -s * mapUnitSize + offset), center + new Vector3(i * mapUnitSize + offset, -1f, s * mapUnitSize - offset));
                }
            }
        }
    }

    internal class UserOperateInfo
    {
        internal static bool mouseDown;
        internal static int mouseLeftClickControlID;
        internal static int mouseLeftUpControlID;
        internal static int selectedLayer;
        internal static int selectedMapItem = 0;

        internal static bool newLayer = false;
        internal static bool editLayer = false;

        internal static bool autoClearOverlapCube = false;
        internal static bool replaceItem = true;//false: not draw

        internal static string currentLayerName = "";
        internal static int currentLayerHeight = 0;
        internal static void SetMouseDown(out int controlID, out Event e)
        {
            controlID = GUIUtility.GetControlID(FocusType.Passive); //獲取操作的行為(如按下 or 放開)
            e = Event.current;
            if (e.alt) return;//alt被按下表示使用者要旋轉畫面，不要進行繪製。
            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown://開始繪製
                    if (e.button != 0) break;
                    GUIUtility.hotControl = controlID;//鎖定滑鼠當前事件，此時按下滑鼠的其他按鈕將不會有反應，也就是說只會接收此控件的所有事件((好像不太好用？)
                    mouseLeftClickControlID = controlID;
                    mouseDown = true;
                    e.Use();//當你在處理Unity的事件系統時（如Update、OnGUI等），可以通過調用Event.Use()方法來標記事件為已使用，這意味著事件將停止傳遞給其他監聽器或系統。
                    break;
                case EventType.MouseUp://停止繪製
                    if (e.button != 0)
                    {
                        if (mouseDown == false) break;
                        Debug.Log(GUIUtility.hotControl);
                        GUIUtility.hotControl = mouseLeftClickControlID;//其他按鍵的MouseUp事件，不要理會他。
                        Debug.Log(GUIUtility.hotControl);
                        break;
                    }
                    else
                    {
                        GUIUtility.hotControl = 0;//等於 0 時，就表示可以接收其他控件的訊號了
                        mouseLeftUpControlID = controlID;
                        mouseDown = false;
                        e.Use();
                        break;
                    }
                #region 沒用到的事件
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                case EventType.Repaint:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.ContextClick:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                case EventType.TouchDown:
                case EventType.TouchUp:
                case EventType.TouchMove:
                case EventType.TouchEnter:
                case EventType.TouchLeave:
                case EventType.TouchStationary:
                default:
                    break;
                    #endregion
            }
        }
    }

    public class ThreeDCubeTileMenu
    {
        [MenuItem("RonTool/3D Tile Map Editor 2022", false, 0)]
        static void OpenControlPanel()
        {
            EditorWindow.GetWindow(typeof(ThreeDCubeTileEditor));
        }
    }


    public class TilesStruct
    {
        public Dictionary<int, GameObject> Layers;//
    }
}