using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChipManager : MonoBehaviour
{
    private static List<Chip> ChipPrefabs;

    public static Chip CreateRandomChip()
    {
        var random = Random.Range(0, ChipPrefabs.Count);
        return CreateChipOfType(random);
    }

    /// <summary>
    /// Если у клетки предустановленное значение, спавнит конкретную фишку.
    /// Иначе не создает фишку.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="y"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public static Chip CreateChipByMapValue(string[] map, int y, int x)
    {
        var value = map[y][x];
        var number = (int)char.GetNumericValue(value);

        if ((number >= 0) && (number < ChipPrefabs.Count))
        {
            return CreateChipOfType(number, true);
        }
        else
        {
            return null;   
        }
    }

    public static Chip CreateChipPseudoRandom(int y, int x)
    {
        var availableChipTypes = new List<int>();

        foreach (var chip in ChipPrefabs)
        {
            availableChipTypes.Add(chip.Type);
        }
               
        var leftChip = BoardManager.Tiles[y, x].LeftTile?.Chip;
        if (leftChip != null)
        {
            availableChipTypes.Remove(leftChip.Type);
        }

        var rightChip = BoardManager.Tiles[y, x].RightTile?.Chip;
        if (rightChip != null)
        {
            availableChipTypes.Remove(rightChip.Type);
        }

        var upChip = BoardManager.Tiles[y, x].UpTile?.Chip;
        if (upChip != null)
        {
            availableChipTypes.Remove(upChip.Type);
        }

        var downChip = BoardManager.Tiles[y, x].DownTile?.Chip;
        if (downChip != null)
        {
            availableChipTypes.Remove(downChip.Type);
        }

        if (availableChipTypes.Count == 0)
        {
            Debug.LogError("Нет такого типа фишек, которого нет рядом с тайлом.");
        }
        var randomRange = Random.Range(0, availableChipTypes.Count);
        var type = availableChipTypes[randomRange];

        return CreateChipOfType(type);
    }
    private static Chip CreateChipOfType(int type, bool isPreset = false)
    {   
        var chip = Instantiate(ChipPrefabs[type]);
        chip.Type = type;
        if (isPreset)
        {
            chip.IsPreset = true;
        }
        return chip;        
    }

    private void Awake()
    {
        ChipPrefabs = new List<Chip>();
        for (var i = 0; i < 5; i++)
        {
            ChipPrefabs.Add(Resources.Load<Chip>($"Prefabs/Chip{i}"));
        }
    }
}
