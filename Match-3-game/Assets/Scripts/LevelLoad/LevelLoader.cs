using System;
using UnityEditor;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private static void LoadNewLevel()
    {
        RemoveLevel();
        LevelDataLoader.FullPath = EditorUtility
            .OpenFilePanel("Select level file", Application.streamingAssetsPath, "json");
        Debug.Log(LevelDataLoader.FullPath);
        LoadLevel();
    }

    private static void RestartLevel()
    {
        foreach (var tile in BoardManager.Tiles)
        {
            tile?.DestroyChip();
        }        
        FillByPresetChips(LevelDataLoader.LevelData.ChipMap);
        FillByPseudoRandomChips();
        RemoveMatches();
        BoardManager.ScoreAndFallUntilSettle();
    }
    
    private static void RemoveLevel()
    {
        for (var y = 0; y < BoardManager.BoardSize; y++)
        for (var x = 0; x < BoardManager.BoardSize; x++)
        {
            var tile = BoardManager.Tiles[y, x];
            tile?.DestroyWithChildren();
            BoardManager.Tiles[y, x] = null;
        }
    }
    
    private static void LoadLevel()
    {
        LevelDataLoader.LoadLevelData(LevelDataLoader.FullPath);
        foreach (var row in LevelDataLoader.LevelData.TileMap)
        {
            if (row.Length != BoardManager.BoardSize)
            {
                throw new Exception("Файл уровня поврежден.");
            }
        }
        if (LevelDataLoader.LevelData.TileMap.Length != BoardManager.BoardSize)
        {
            throw new Exception("Файл уровня поврежден.");
        }
        foreach (var row in LevelDataLoader.LevelData.ChipMap)
        {
            if (row.Length != BoardManager.BoardSize)
            {
                throw new Exception("Файл уровня поврежден.");
            }
        }
        if (LevelDataLoader.LevelData.ChipMap.Length != BoardManager.BoardSize)
        {
            throw new Exception("Файл уровня поврежден.");
        }

        CreateTileGrid(LevelDataLoader.LevelData.TileMap);
        FillByPresetChips(LevelDataLoader.LevelData.ChipMap);
        FillByPseudoRandomChips();
        RemoveMatches();
        BoardManager.ScoreAndFallUntilSettle();
    }
    
    private static void CreateTileGrid(string[] tileMap)
    {
        for (var y = 0; y < tileMap.Length; y++)
        { 
            for (var x = 0; x < tileMap[y].Length; x++)
            {
                BoardManager.Tiles[y,x] = TileManager.CreateTileByMapValue(tileMap[y][x], y, x);
            }
        }
    }
    
    private static void FillByPresetChips(string[] chipMap)
    {
        for (var y = 0; y < BoardManager.BoardSize; y++)
        for (var x = 0; x < BoardManager.BoardSize; x++)
        {
            var tile = BoardManager.Tiles[y, x];
            tile?.SetChip(ChipManager.CreateChipByMapValue(chipMap,y,x));
        }
    }

    private static void FillByPseudoRandomChips()
    {
        for (var y = 0; y < BoardManager.BoardSize; y++)
        for (var x = 0; x < BoardManager.BoardSize; x++)
        {
            var tile = BoardManager.Tiles[y, x];
            if (tile?.Chip != null)
            {
                continue;
            }
            tile?.SetChip(ChipManager.CreateChipPseudoRandom(y, x));
        }
    }
    private static void RemoveMatches()
    {
        foreach (var tile in BoardManager.Tiles)
        {
            var chip = tile?.Chip;
            if ((chip?.IsPreset == false) && MatchFinder.IsInMatch(chip))
            {
                tile.DestroyChip();
                tile.SetChip(ChipManager.CreateChipPseudoRandom(tile.y, tile.x));
            }
        }
    }
    
    private void OnGUI()
    {
        if (GUI.Button(new Rect(30, 50, 110, 40), "Load new level"))
        {
            LoadNewLevel();
        }
        if (LevelDataLoader.IsLevelLoaded)
        {
            if (GUI.Button(new Rect(30, 100, 110, 40), "Restart level"))
            {
                RestartLevel();
            }
        }
    }
}
