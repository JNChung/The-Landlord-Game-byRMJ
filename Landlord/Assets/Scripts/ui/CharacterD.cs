using Ron.Base.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterD : MonoBehaviour
{
    Character character;
    public bool Selected;
    bool showPathDone;
    // Start is called before the first frame update
    void Start()
    {
        Selected = false;
        InitialPosition();
        character = new Character(100, transform.position.ToV3Int(), 3);
    }

    private void InitialPosition()
    {
        var newPos = transform.position.ToV3Int();
        transform.position = newPos;
    }

    // Update is called once per frame
    void Update()
    {
        ShowPath();
    }

    void ShowPath()
    {
        if (Selected && showPathDone == false)
        {
            showPathDone = true;
            var paths = character.CanMoveTiles(character.Speed);
            foreach (var path in paths)
            {
                path.Current.ShowPath();
            }
        }
    }
}
