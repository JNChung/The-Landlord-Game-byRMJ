using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using RonTools;
using Codice.Utils;
using UnityEngine.Tilemaps;

namespace Ron.Tools
{
    public class ThreeDCubeTileMenu
    {
        [MenuItem("RonTool/3D Tile Map Editor 2022", false, 0)]
        static void OpenControlPanel()
        {
            EditorWindow.GetWindow(typeof(ThreeDCubeTileEditor));
        }
    }
    public class ThreeDCubeTileEditor : EditorWindow
    {
        static GameObject tileMap;
        GameObject TileMap
        {
            set { tileMap = value; }
            get
            {
                if (tileMap != null) return tileMap;
                CleanLayerContainer();
                tileMap = GameObject.Find("TileMap");
                if (tileMap != null)
                {
                    Transform[] trs = tileMap.GetComponentsInChildren<Transform>();
                    foreach (var item in trs)
                    {
                        if (item.parent == tileMap.transform)//第一層子物件
                        {
                            AddNewLayer(item.gameObject);
                        }

                        if (layerObjs.Count == 0)
                        {
                            AddDefaultLayer();
                        }
                    }
                    return tileMap;
                }
                else
                {
                    tileMap = new GameObject();
                    tileMap.name = "TileMap";

                    AddDefaultLayer();

                    return tileMap;
                }
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

        //地圖繪製區變數
        bool mouseDown = false;
        static int mapUnitSize = 1;
        static Color cursorColor = Color.yellow;
        static bool showGrid = true;
        static int gridCount = 9;//格線數量，太多會影響效能
        static Color gridColor = Color.gray;
        static bool autoClearOverlapCube = false;
        static bool replaceItem = true;//false: not draw
        static List<Dictionary<Vector3Int, GameObject>> mapDics = new List<Dictionary<Vector3Int, GameObject>>();
        static bool onPainting = false;

        //存檔區變數
        static string tempFilename = "";
        private void OnGUI()
        {
            Input.imeCompositionMode = IMECompositionMode.On;//讓輸入單元支援中文。
            Texture2D result = new Texture2D(1, 1);//加入這一行才能即時顯示格線。
            UpdateContentWidth();
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);//box要用的風格檔。
            boxStyle.normal.textColor = Color.white;
            boxStyle.fixedWidth = contentWidth;
            GUILayout.Label("我是中文編輯器");
            scrollPos = Nested._(() => EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(contentWidth)), () =>
            {
                //方塊管理區
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    GUILayout.Space(10);//加上一排空格
                    GUILayout.Box("地圖元件", boxStyle);
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (GUILayout.Button("加入地圖元件")) { AddMapItem(); }
                        if (GUILayout.Button("刪除地圖元件")) { RemoveMapItem(); }
                        if (GUILayout.Button("刪除全圖元件")) { RemoveAllMapItem(); }
                        //顯示尺寸用的滑桿
                        iconSize = GUILayout.HorizontalSlider(iconSize, 40, 120, GUILayout.Width(60));
                    }, () => GUILayout.EndHorizontal());
                }, () => GUILayout.EndVertical());
                //顯示地圖元件列表
                DisplayMapItems();
                //圖層管理區
                Nested._(() => GUILayout.BeginVertical(GUILayout.Width(contentWidth)), () =>
                {
                    GUILayout.Space(10);
                    GUILayout.Box("圖層", boxStyle);
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (!newLayer && !editLayer)//新增和編輯圖層就不顯示按鈕
                        {
                            if (GUILayout.Button("新增圖層")) { }
                            if (GUILayout.Button("編輯圖層")) { }
                            if (GUILayout.Button("刪除圖層")) { }
                        }
                        if (newLayer)
                        {

                        }
                        else//editLayer
                        {

                        }
                    }, () => GUILayout.EndHorizontal());
                }, () => GUILayout.EndVertical());
                //地圖製作區
                Nested._(() => GUILayout.BeginVertical(), () =>
                {
                    GUILayout.Space(10);
                    GUILayout.Box("地圖製作", boxStyle);
                    //設定單位尺寸
                    //設定格線尺寸
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        autoClearOverlapCube = GUILayout.Toggle(autoClearOverlapCube, "自動清除不同圖層重疊方塊");
                        replaceItem = GUILayout.Toggle(replaceItem, "自動取代");
                    }, () => GUILayout.EndHorizontal());
                    if (onPainting) { }
                    else { }

                    GUILayout.Space(10);
                    GUILayout.Box("檔案", boxStyle);
                    Nested._(() => GUILayout.BeginHorizontal(), () =>
                    {
                        if (GUILayout.Button("新地圖")) { }
                        if (GUILayout.Button("儲存檔案")) { }
                        if (GUILayout.Button("載入舊檔")) { }
                    }, () => GUILayout.EndHorizontal());

                }, () => GUILayout.EndVertical());
            }, () => EditorGUILayout.EndScrollView());
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
        void RebuildMapDic()
        {

        }
        void SaveData(string filename) { }
        void LoadData(string filename) { }
        Vector3Int V3ToV3Int(Vector3 val)
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
            var t = TileMap;
            RebuildMapDic();
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += this.OnSceneGUI;
        }
        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }
        // SceneView 繪製用函式
        [DrawGizmo(GizmoType.NonSelected)]
        static void RenderGridGizmo(Transform objectTransform, GizmoType gizmoType)
        {
            if (layerObjs.Count == 0 || tileMap == null) return;
            if (showGrid)
            {
                int s = (gridCount + 1) / 2;
                int h = Mathf.RoundToInt(layerObjs[selectedLayer].transform.position.y);
                Gizmos.color = gridColor;
                float offset = 0.5f * mapUnitSize;
                for (int i = -s; i < s; i++)
                {
                    Gizmos.DrawLine(new Vector3(-s * mapUnitSize + offset, h, i * mapUnitSize + offset),
                        new Vector3(s * mapUnitSize - offset, h, i * mapUnitSize + offset));
                    Gizmos.DrawLine(new Vector3(i * mapUnitSize + offset, h, -s * mapUnitSize + offset), new Vector3(i * mapUnitSize + offset, h, s * mapUnitSize - offset));
                }
            }
        }
        void OnSceneGUI(SceneView sceneView)//繪製各種UI物件
        {
            //Handles.BeginGUI();
            //{
            //   若有需要在 SceneView 繪製各種UI功能
            //}
            //Handles.EndGUI();
            //onPainting = true;  //測試用
            if (!onPainting) return;
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
            int dx = 0, dy = 0, dz = 0;
            bool mouseInWorld = MouseToWorldPosition(e.mousePosition, Mathf.RoundToInt(layerObjs[selectedLayer].transform.position.y), out pos);

            //繪製游標
            if (mouseInWorld)
            {
                posInt = V3ToV3Int(pos);
                dy = posInt.y;
                dx = (posInt.x / mapUnitSize) * mapUnitSize; //貼齊格線小技巧
                dz = (posInt.z / mapUnitSize) * mapUnitSize;
                posInt = new Vector3Int(dx, posInt.y, dz);

                float cursorOffset = (float)mapUnitSize / 2f;
                Vector3 p1 = new Vector3(posInt.x - cursorOffset, posInt.y, posInt.z - cursorOffset);
                Vector3 p2 = new Vector3(posInt.x - cursorOffset, posInt.y, posInt.z + cursorOffset);
                Vector3 p3 = new Vector3(posInt.x + cursorOffset, posInt.y, posInt.z + cursorOffset);
                Vector3 p4 = new Vector3(posInt.x + cursorOffset, posInt.y, posInt.z - cursorOffset);

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
            if (mouseDown)
            {
                if (mouseInWorld)
                {
                    Vector3Int dicPos = new Vector3Int(dx, 0, dz);
                    if (e.shift)//刪除方塊
                    {
                        GameObject goToBeDeleted;
                        if (mapDics[selectedLayer].TryGetValue(dicPos, out goToBeDeleted))
                        {
                            mapDics[selectedLayer].Remove(dicPos);
                            DestroyImmediate(goToBeDeleted);
                        }
                    }
                    else//繪製方塊
                    {
                        if (mapDics[selectedLayer].ContainsKey(dicPos) && !replaceItem) return;

                        if (autoClearOverlapCube)
                        {
                            foreach (var dic in mapDics)
                            {
                                if (dic.ContainsKey(dicPos))
                                {
                                    GameObject goToBeDeleted;
                                    if(dic.TryGetValue(dicPos, out goToBeDeleted))
                                    {
                                        dic.Remove(dicPos);
                                        DestroyImmediate(goToBeDeleted);
                                    }
                                }
                            }
                        }

                        if (mapDics[selectedLayer].ContainsKey(dicPos))
                        {
                            GameObject goToBeDeleted;
                            if (mapDics[selectedLayer].TryGetValue(dicPos, out goToBeDeleted))
                            {
                                mapDics[selectedLayer].Remove(dicPos);
                                DestroyImmediate(goToBeDeleted);
                            }
                        }

                        //繪製方塊
                        GameObject cube = (GameObject)PrefabUtility.InstantiatePrefab(mapItemPrefabs[selectedNum]);
                        cube.transform.position = posInt;
                        cube.transform.SetParent(layerObjs[selectedLayer].transform);

                        mapDics[selectedLayer].Add(dicPos, cube);
                    }
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

        void DisplayMapItems()
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
    }
}