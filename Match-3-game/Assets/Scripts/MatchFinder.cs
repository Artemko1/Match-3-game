using System;
using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    private const int RowToScore = 3;

    /// <summary>
    /// Возвращает true если фишка стоит
    /// в ряду хотя бы из 3 таких же фишек
    /// </summary>
    /// <param name="chip"></param>
    /// <returns></returns>
    public static bool IsInMatch(Chip chip)
    {
        return (FindHorizontalMatch(chip).Count + 1 >= RowToScore || FindVerticalMatch(chip).Count + 1 >= RowToScore);
    }
    
    /// <summary>
    /// Возвращает фишки наибольшего матча среди фишек, 
    /// находящихся в матче с переданной.
    /// </summary>
    /// <param name="chip"></param>
    /// <returns></returns>
    public static List<Chip> GetMaxMatchChips(Chip chip)
    {
        var chipsInMatch = GetMatchChips(chip);
        var maxScore = 0;
        List<Chip> maxChips = null;
        foreach (var chipInMatch in chipsInMatch)
        {
            var currentChips = GetMatchChips(chipInMatch);
            var currentScore = currentChips.Count;
            if (currentScore > maxScore)
            {
                maxScore = currentScore;
                maxChips = currentChips;
            }
        }
        return maxChips;
    }


    /// <summary>
    /// Возвращает список фишек, находящихся в матче с переданной.
    /// </summary>
    /// <param name="chip"></param>
    /// <returns></returns>
    private static List<Chip> GetMatchChips(Chip chip)
    {
        if (chip == null)
        {
            Debug.LogWarning("Finding matches for null chip.");
            return null;
        }
        var list = new List<Chip>();        
        var horizontalList = FindHorizontalMatch(chip);
        var verticalList = FindVerticalMatch(chip);

        if (horizontalList.Count + 1 >= 3)
        {
            list.AddRange(horizontalList);
        }
        if (verticalList.Count + 1 >= 3)
        {
            list.AddRange(verticalList);
        }
        if (horizontalList.Count + 1 >= 3 || verticalList.Count + 1 >= 3)
        {
            list.Add(chip);
        }
        return list;
    }
    
    /// <summary>
    /// Возвращает фишки того же типа слева и справа от переданной
    /// </summary>
    /// <param name="chip"></param>
    /// <returns></returns>
    private static List<Chip> FindHorizontalMatch(Chip chip)
    {
        var list = new List<Chip>();
        list.AddRange(CountChipRow(chip, chip2 => chip2.LeftChip));
        list.AddRange(CountChipRow(chip, chip2 => chip2.RightChip));
        return list;
    }
    /// <summary>
    /// Возвращает фишки того же типа сверху и снизу от переданной
    /// </summary>
    /// <param name="chip"></param>
    /// <returns></returns>
    private static List<Chip> FindVerticalMatch(Chip chip)
    {
        var list = new List<Chip>();
        list.AddRange(CountChipRow(chip, chip2 => chip2.UpChip));
        list.AddRange(CountChipRow(chip, chip2 => chip2.DownChip));
        return list;
    }

    /// <summary>
    /// Возвращает фишки того же типа что chip в указанном ряду
    /// </summary>
    /// <param name="chip"></param>
    /// <param name="getNextChip"></param>
    /// <returns></returns>
    private static List<Chip> CountChipRow(Chip chip, Func<Chip, Chip> getNextChip)
    {
        var list = new List<Chip>();
        var secondChip = getNextChip.Invoke(chip);
        if (secondChip == null || chip.Type != secondChip.Type)
        {
            return list;
        }
        else
        {
            list.Add(secondChip);
            list.AddRange(CountChipRow(secondChip, getNextChip));
            return list;
        }
    }
}
