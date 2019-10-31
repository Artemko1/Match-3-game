using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public const float TileSize = 0.64f;

    private static GameObject TilePrefab;

    public static Tile CreateTileByMapValue(char value, int y, int x)
    {
        var number = (int)char.GetNumericValue(value);
        Tile tile = null;

        
        switch (number)
        {
            case 0:
                tile = CreateTile(y, x);
                break;
            case 1:
                tile = CreateTile(y, x, true);
                break;
        }
        return tile;
    }

    private static Tile CreateTile(int y, int x, bool createEmitter = false)
    {
        var tile = Instantiate(TilePrefab).GetComponent<Tile>();

        var tilePos = Vector2.one;
        tilePos.x *= x * TileSize;
        tilePos.y *= -y * TileSize;
        tile.transform.position = tilePos;

        tile.y = y;
        tile.x = x;

        tile.name = $"Tile {y}, {x}";

        if (createEmitter)
        {
            tile.CreateEmitter();
        }
        return tile;
    }

    private void Awake()
    {
        TilePrefab =  Resources.Load<GameObject>("Prefabs/Tile");
    }
}
