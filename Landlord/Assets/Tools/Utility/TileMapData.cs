using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
namespace Ron.Tools
{
    [Serializable]
    public class TileMapData
    {
        public Vector3 camPos;
        public Vector3 camRot;
        public float camFDV;
        public Vector3Int playerPos;
        public Vector3 playerRot;

        public string[] resBlocks;
        public TileMapLayer[] layerDatas;
    }

    [Serializable]
    public struct TileMapLayer
    {
        public string name;
        public int height;
        public List<Block> blocks;
    }

    [Serializable]
    public struct BlockPair
    {
        public List<string> resources;
        public List<Block> blocks;
    }
    [Serializable]
    public struct Block
    {
        public int index;
        public Vector3Int pos;

        public Block(int Index, Vector3Int Pos)
        {
            this.index = Index;
            this.pos = Pos;
        }
    }
}