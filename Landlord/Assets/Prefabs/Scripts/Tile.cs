using Ron.Base.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour
{
    public TileData TileData { get; private set; }
    public string Type;
    // Start is called before the first frame update
    void Start()
    {
        TileData = new TileData(this.transform.position.ToV3Int(), Type);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
