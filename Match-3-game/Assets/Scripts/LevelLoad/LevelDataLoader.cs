using System.IO;
using UnityEngine;

public class LevelDataLoader
{
    public static LevelData LevelData;
    public static string FullPath;

    public static bool IsLevelLoaded { get; private set; }

    private static string RelativePath = "TestLevel.json";

    public static void LoadLevelData(string fullPath)
    {
        if (File.Exists(fullPath))
        {
            var json = File.ReadAllText(fullPath);
            LevelData = JsonUtility.FromJson<LevelData>(json);
            IsLevelLoaded = true;
            Debug.Log("Level loaded");
        }
        else
        {
            Debug.LogError("No json file at path " + fullPath);
        }
    }
    
    [ContextMenu("Generate simple level")]
    private void GenerateSimpleLevel()
    {
        GenerateLevelData();

        var filePath = Path.Combine(Application.dataPath, RelativePath);
        File.WriteAllText(filePath, JsonUtility.ToJson(LevelData, true));
        Debug.Log("Saved to " + filePath);
    }

    private void GenerateLevelData()
    {       
        string[] tileMap =
        {
            "222222222" ,
            "001111100" ,
            "001111100" ,
            "001111100" ,
            "001111100" ,
            "001111100" ,
            "001111100" ,
            "221111122" ,
            "111111111" ,
        };

        string[] chipMap =
       {
            "111111111" ,
            "111111111" ,
            "111111111" ,
            "111111111" ,
            "111111111" ,
            "111111111" ,
            "000000000" ,
            "000000000" ,
            "121212121" ,
        };

        LevelData = new LevelData() { TileMap = tileMap, ChipMap = chipMap };
    }
}
