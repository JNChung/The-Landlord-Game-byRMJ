using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;
using RonTools;
using Codice.Utils;

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
        Vector2 scrollPos;
        float contentWidth;

        //地圖方塊區變數
        string filepath = "Tools/Utility/Editor/Datas/MapItems.json";
        List<GameObject> mapItemPrefabs = new List<GameObject>();
        List<string> mapItemNames = new List<string>();
        List<Texture> mapItemIcons = new List<Texture>();
        int selectedNum = 0;
        float iconSize = 80;//縮圖尺寸
        private string tempPath = "";

        //地圖方塊區變數
        //地圖方塊區變數

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
                //地圖製作區
            }, () => EditorGUILayout.EndScrollView());
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
            SaveAndLoad saveAndLoad = new SaveAndLoad();
            saveAndLoad.SaveJson(new MapItem(mapItemNames.ToArray()));
        }
        void RebuildMapItemData()
        {
            mapItemPrefabs.Clear();
            mapItemNames.Clear();
            mapItemIcons.Clear();
            selectedNum = 0;

            SaveAndLoad saveAndLoad = new SaveAndLoad();
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