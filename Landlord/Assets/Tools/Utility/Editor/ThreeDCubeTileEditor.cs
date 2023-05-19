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
        void OnSceneGUI(SceneView sceneView)//繪製各種UI物件
        {
            //Handles.BeginGUI();
            //{
            //   若有需要在 SceneView 繪製各種UI功能
            //}
            //Handles.EndGUI();
            //onPainting = true;  //測試用
            if (onPainting == false) return;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event e = Event.current;
            if (e.alt) return;//alt被按下表示使用者要旋轉畫面，不要進行繪製。
            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown://開始繪製
                    if (e.button == 0)
                    {
                        GUIUtility.hotControl = controlID;
                        mouseDown = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp://停止繪製
                    if (e.button != 0) break;
                    GUIUtility.hotControl = 0;
                    mouseDown = false;
                    Event.current.Use();
                    break;
                case EventType.MouseMove:
                    break;
                case EventType.MouseDrag:
                    break;
                case EventType.KeyDown:
                    break;
                case EventType.KeyUp:
                    break;
                case EventType.ScrollWheel:
                    break;
                case EventType.Repaint:
                    break;
                case EventType.Layout:
                    break;
                case EventType.DragUpdated:
                    break;
                case EventType.DragPerform:
                    break;
                case EventType.DragExited:
                    break;
                case EventType.Ignore:
                    break;
                case EventType.Used:
                    break;
                case EventType.ValidateCommand:
                    break;
                case EventType.ExecuteCommand:
                    break;
                case EventType.ContextClick:
                    break;
                case EventType.MouseEnterWindow:
                    break;
                case EventType.MouseLeaveWindow:
                    break;
                case EventType.TouchDown:
                    break;
                case EventType.TouchUp:
                    break;
                case EventType.TouchMove:
                    break;
                case EventType.TouchEnter:
                    break;
                case EventType.TouchLeave:
                    break;
                case EventType.TouchStationary:
                    break;
                default:
                    break;
            }
            var pos = Vector3.zero;
            Vector3Int posInt = Vector3Int.zero;
            bool mouseInWorld = MouseToWorldPosition(e.mousePosition, Mathf.RoundToInt(layerObjs[selectedLayer].transform.position.y), out pos);

            if (mouseInWorld)
            {
                posInt = V3ToV3Int(pos);
                PaintGridCursor(posInt);
            }
            if (mouseDown)
            {
                if (mouseInWorld)
                {
                    Vector3Int key = posInt.WithY();
                    if (e.shift)//刪除方塊
                    {
                        DeleteTile(key);
                    }
                    else//繪製方塊
                    {
                        if (autoClearOverlapCube)
                        {
                            foreach (var dic in mapDics)
                            {
                                DeleteTile(key, dic);
                            }
                        }

                        if (mapDics[selectedLayer].ContainsKey(key) && replaceItem)
                        {
                            DeleteTile(key);
                        }

                        //繪製方塊
                        GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(mapItemPrefabs[selectedNum]);
                        cube.transform.position = posInt;
                        cube.transform.SetParent(layerObjs[selectedLayer].transform);
                        mapDics[selectedLayer].Add(key, cube);
                    }
                }
            }
        }
        #region OnSceneGUI 專用
        void DeleteTile(Vector3Int key)
        {
            GameObject goToBeDeleted;
            if (mapDics[selectedLayer].TryGetValue(key, out goToBeDeleted))
            {
                mapDics[selectedLayer].Remove(key);
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

        private void OnGUI()
        {
            //初始化
            ChineseSupport();
            //Texture2D result = new Texture2D(1, 1);//加入這一行才能即時顯示格線。
            UpdateContentWidth();
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);//box要用的風格檔。
            boxStyle.normal.textColor = Color.white;
            boxStyle.fixedWidth = contentWidth;

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
                        if (!newLayer && !editLayer)//彈窗效果：不是新增和編輯圖層就不顯示按鈕
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
                        if (newLayer)
                        {
                            AddMapLayerImplement();
                        }
                        else if (editLayer)//editLayer
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
                        autoClearOverlapCube = GUILayout.Toggle(autoClearOverlapCube, "自動清除不同圖層重疊方塊");
                        replaceItem = GUILayout.Toggle(replaceItem, "自動取代");
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
        }



        private void AddDefaultLayer()
        {
            GameObject go = new GameObject();
            go.name = "Default";
            go.transform.SetParent(TileMap.transform);
            AddNewLayer(go);
            selectedLayer = 0;
        }

        private void AddNewLayer(GameObject go)
        {
            layerObjs.Add(go);
            layerNames.Add(go.name);
            mapDics.Add(new Dictionary<Vector3Int, GameObject>());//建立圖層時，要順便建立該圖層的字典容器。
        }

        private void CleanLayerContainer()
        {
            layerObjs.Clear();
            layerNames.Clear();
            mapDics.Clear();
        }

        Vector2 scrollPos;
        float contentWidth;

        //地圖方塊區變數
        string filepath = "Tools/Utility/Editor/Datas/MapItems.json";
        static List<GameObject> mapItemPrefabs = new List<GameObject>();
        static List<string> mapItemNames = new List<string>();
        static List<Texture> mapItemIcons = new List<Texture>();
        static int selectedNum = 0;
        static float iconSize = 80;//縮圖尺寸
        private string tempPath = "";

        //圖層變數
        static List<GameObject> layerObjs = new List<GameObject>();
        static List<string> layerNames = new List<string>();
        static bool newLayer = false;
        static bool editLayer = false;
        static string layerName = "";
        static int layerHeight = 0;
        static int selectedLayer = 0;


        //存檔區變數
        static string tempFilename = "";
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
        void RebuildMapDic()
        {
            Debug.LogWarning("重建字典...");
            if (tileMap == null)
            {
                Debug.LogWarning("重建字典失敗，物件不存在");
                return;
            }

            for (int i = 0; i < layerNames.Count(); i++)
            {
                mapDics[i].Clear();
                Transform[] children = layerObjs[i].GetComponentsInChildren<Transform>();
                foreach (var item in children)
                {
                    if (item.parent == layerObjs[i].transform)
                    {
                        Vector3Int posInt = V3ToV3Int(item.transform.position);
                        Vector3Int dicKey = posInt.WithY();
                        item.transform.position = posInt;//對齊方塊
                        if (!mapDics[i].ContainsKey(dicKey))
                        {
                            mapDics[i].Add(dicKey, item.gameObject);
                        }
                    }
                }
            }
        }
        void SaveData(string filename)
        {
            TileMapData data = new TileMapData();
            RebuildMapItemData();
            RebuildTileMap();
            RebuildMapDic();

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                data.camPos = cam.position;
                data.camRot = cam.eulerAngles;
                data.camFDV = cam.GetComponent<Camera>().fieldOfView;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                data.playerPos = V3ToV3Int(player.transform.position);
                data.playerRot = player.transform.eulerAngles;
            }

            data.resBlocks = mapItemNames.ToArray();
            data.layerDatas = new TileMapLayer[layerObjs.Count];

            for (int i = 0; i < layerObjs.Count; i++)
            {
                data.layerDatas[i].name = layerObjs[i].name;
                data.layerDatas[i].height = Mathf.RoundToInt(layerObjs[i].transform.position.y);
                data.layerDatas[i].blocks = new List<Block>();

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
                        for (int j = 0; j < data.resBlocks.Length; j++)
                        {
                            if (assetPath.ToLower() == data.resBlocks[j].ToLower())
                            {
                                index = j;
                                break;
                            }

                        }

                        data.layerDatas[i].blocks.Add(new Block(index, V3ToV3Int(item.position)));
                    }
                }

            }

            SaveAndLoad saveAndLoad = new SaveAndLoad(filename);
            saveAndLoad.SaveJson(data);
        }
        void LoadData(string filename)
        {
            TileMapData data = new TileMapData();
            SaveAndLoad saveAndLoad = new SaveAndLoad(filename);
            data = saveAndLoad.LoadJson<TileMapData>();

            Transform cam = Camera.main.transform;
            if (cam != null)
            {
                cam.position = data.camPos;
                cam.eulerAngles = data.camRot;
                Camera.main.GetComponent<Camera>().fieldOfView = data.camFDV;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = data.playerPos;
                player.transform.eulerAngles = data.playerRot;
            }

            //清空資源列表
            mapItemPrefabs.Clear();
            mapItemNames.Clear();
            mapItemIcons.Clear();
            selectedNum = 0;
            CleanLayerContainer();
            DestroyImmediate(tileMap);

            //讀取資源列表
            foreach (var item in data.resBlocks)
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
            foreach (var item in data.layerDatas)
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
                    mapDics[dicNum].Add(dicPos, ob);
                }
                dicNum++;
            }

        }
        static Vector3Int V3ToV3Int(Vector3 val)
        {
            int x = Mathf.RoundToInt(val.x);
            int y = Mathf.RoundToInt(val.y);
            int z = Mathf.RoundToInt(val.z);
            return new Vector3Int(x, y, z);
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
        //系統用函式
        private void OnFocus()
        {
            RebuildMapItemData();
            RebuildTileMap();
            RebuildMapDic();
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


        //地圖繪製區變數
        bool mouseDown = false;
        static int mapUnitSize = 1;
        static Color cursorColor = Color.yellow;
        static bool showGrid = true;
        static int gridCount = 19;//格線數量，太多會影響效能
        static Color gridColor = Color.gray;
        static bool autoClearOverlapCube = false;
        static bool replaceItem = true;//false: not draw
        static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();
        static bool onPainting = false;
        Vector3Int camCenter;
        // SceneView 繪製用函式
        [DrawGizmo(GizmoType.NonSelected)]
        static void RenderGridGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (layerObjs.Count == 0 || tileMap == null) return;
            if (showGrid)
            {
                int s = (gridCount + 1) / 2;
                int h = Mathf.RoundToInt(layerObjs[selectedLayer].transform.position.y);

                Transform cam = SceneView.currentDrawingSceneView.camera.transform;
                Ray cf = new Ray(cam.position, cam.forward);
                Plane plane = new Plane(Vector3.up, new Vector3(0, h, 0));
                if (!plane.Raycast(cf, out float enter)) return;
                Vector3Int center = V3ToV3Int(cf.GetPoint(enter));
                Gizmos.color = gridColor;
                float offset = 0.5f * mapUnitSize;
                for (int i = -s; i < s; i++)
                {
                    Gizmos.DrawLine(center + new Vector3(-s * mapUnitSize + offset, -0.5f, i * mapUnitSize + offset), center +
                        new Vector3(s * mapUnitSize - offset, -0.5f, i * mapUnitSize + offset));
                    Gizmos.DrawLine(center + new Vector3(i * mapUnitSize + offset, -0.5f, -s * mapUnitSize + offset), center + new Vector3(i * mapUnitSize + offset, -0.5f, s * mapUnitSize - offset));
                }
            }
        }


        //地圖元件用的函式
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
            SaveAndLoad saveAndLoad = new SaveAndLoad(filepath);
            saveAndLoad.SaveJson(new MapItem(mapItemNames.ToArray()));
        }
        void RebuildMapItemData()
        {
            mapItemPrefabs.Clear();
            mapItemNames.Clear();
            mapItemIcons.Clear();
            selectedNum = 0;

            SaveAndLoad saveAndLoad = new SaveAndLoad(filepath);
            if (!saveAndLoad.FileExists()) return;
            MapItem data = saveAndLoad.LoadJson<MapItem>();
            foreach (string name in data.names)
            {
                AddMapItemToContainer(name);
            }

            Repaint();//全部重繪，以避免有時候縮圖顯示不出來的問題
        }

        private void AddMapItemToContainer(string name)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(name, typeof(GameObject));
            mapItemPrefabs.Add(go);
            mapItemNames.Add(name);
            mapItemIcons.Add(AssetPreview.GetAssetPreview(go));
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
        void RemoveMapItemFromContainer()
        {
            mapItemPrefabs.Remove(mapItemPrefabs[selectedNum]);
            mapItemNames.Remove(mapItemNames[selectedNum]);
            mapItemIcons.Remove(mapItemIcons[selectedNum]);
        }
        void RemoveMapItem()
        {
            if (mapItemPrefabs.Count > 0)
            {
                if (!EditorUtility.DisplayDialog("刪除地圖元件", $"確定要移除元件「{mapItemNames[selectedNum]}」嗎？", "確定", "取消")) return;

                RemoveMapItemFromContainer();
                SaveMapItemData();
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
                selectedNum = 0;

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
                    selectedNum = GUILayout.SelectionGrid(selectedNum, mapItemIcons.ToArray(), xCount, GUILayout.Width(contentWidth - 20), GUILayout.Height(lines * iconSize));
                }
                else //顯示詳細文字列表
                {
                    selectedNum = GUILayout.SelectionGrid(selectedNum, mapItemNames.ToArray(), 1, GUILayout.Width(contentWidth - 20));
                }
            }
        }
        private string TryGetPastDirectorPath()
        {
            return tempPath;
        }

        private void UpdateContentWidth()
        {
            contentWidth = position.width - 20;
        }

        //圖層用函式
        void AddMapLayer()
        {
            newLayer = true;
            layerName = "New Layer";
            layerHeight = 0;
        }
        void AddMapLayerImplement()
        {
            Nested._(() => GUILayout.BeginVertical(), () =>
            {
                Nested._(() => GUILayout.BeginHorizontal(), () =>
                {
                    GUILayout.Label("圖層名稱", GUILayout.Width(60));
                    layerName = EditorGUILayout.TextField(layerName);
                }, () => GUILayout.EndHorizontal());

                Nested._(() => GUILayout.BeginHorizontal(), () =>
                {
                    GUILayout.Label("圖層高度", GUILayout.Width(60));
                    layerHeight = EditorGUILayout.IntField(layerHeight);
                    if (GUILayout.Button("-")) layerHeight -= 1;
                    if (GUILayout.Button("+")) layerHeight += 1;
                    if (GUILayout.Button("確定"))
                    {
                        GameObject go = new GameObject();
                        go.name = layerName;
                        go.transform.position = new Vector3(0, layerHeight, 0);
                        go.transform.SetParent(TileMap.transform);
                        AddNewLayer(go);
                        newLayer = false;
                        layerName = "";
                    }
                    if (GUILayout.Button("取消"))
                    {
                        newLayer = false;
                        layerName = "";
                    }

                }, () => GUILayout.EndHorizontal());


            }, () => GUILayout.EndVertical());
        }
        void ModifyMapLayer()
        {
            editLayer = true;
            layerName = layerNames[selectedLayer];
            layerHeight = Mathf.RoundToInt(layerObjs[selectedLayer].transform.position.y);
        }
        void ModifyMapLayerImplement()
        {
            Nested._(() => GUILayout.BeginVertical(), () =>
            {
                Nested._(() => GUILayout.BeginHorizontal(), () =>
                {
                    GUILayout.Label("圖層名稱", GUILayout.Width(60));
                    layerName = EditorGUILayout.TextField(layerName);
                }, () => GUILayout.EndHorizontal());

                Nested._(() => GUILayout.BeginHorizontal(), () =>
                {
                    GUILayout.Label("圖層高度", GUILayout.Width(60));
                    layerHeight = EditorGUILayout.IntField(layerHeight);
                    if (GUILayout.Button("-")) layerHeight -= 1;
                    if (GUILayout.Button("+")) layerHeight += 1;
                    if (GUILayout.Button("確定"))
                    {
                        layerObjs[selectedLayer].name = layerName;
                        layerObjs[selectedLayer].transform.position = new Vector3(0, layerHeight, 0);
                        layerNames[selectedLayer] = layerName;
                        editLayer = false;
                        layerName = "";
                    }
                    if (GUILayout.Button("取消"))
                    {
                        editLayer = false;
                        layerName = "";
                    }
                }, () => GUILayout.EndHorizontal());


            }, () => GUILayout.EndVertical());

        }
        void DisplayLayerList()
        {
            selectedLayer = GUILayout.SelectionGrid(selectedLayer, layerNames.ToArray(), 1, GUILayout.Width(contentWidth));
        }
        void RemoveMapLayer()
        {
            if (!EditorUtility.DisplayDialog("刪除圖層", $"確定要刪除圖層「{layerNames[selectedLayer]}」嗎？\n該圖層的資料會全部刪除", "確定", "取消")) return;
            DestroyImmediate(layerObjs[selectedLayer]);//刪除圖層管理的GameObject
            layerObjs.Remove(layerObjs[selectedLayer]);
            layerNames.Remove(layerNames[selectedLayer]);
            mapDics.Remove(mapDics[selectedLayer]);
            selectedLayer = 0;
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
                SaveData(filename);
            }
        }

        void LoadMapData()
        {
            selectedNum = 0;
            selectedLayer = 0;
            string defaultPath = tempPath;
            if (defaultPath == "") defaultPath = Application.dataPath;

            string filename = EditorUtility.OpenFilePanel("載入舊檔", defaultPath, "json");
            if (filename != "")
            {
                tempPath = Path.GetDirectoryName(filename);
                tempFilename = Path.GetFileName(filename);
                LoadData(filename);
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
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
}