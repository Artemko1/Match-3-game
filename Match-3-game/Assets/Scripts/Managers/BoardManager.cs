using UnityEngine;
using DG.Tweening;

public static class BoardManager
{
    public const int BoardSize = 9;

    public static readonly Tile[,] Tiles = new Tile[BoardSize,BoardSize];


    public static Sequence ScoreAndFallUntilSettle()
    {
        bool successScore;
        var seqFullCircle = DOTween.Sequence();
        do
        {
            seqFullCircle
                .Append(ScoreMatches(out successScore))
                .Append(FallAll());

        } while (successScore);
        
        seqFullCircle.onPlay += () => GameStateManager.CurrentState = GameStateManager.GameState.Falling;
        seqFullCircle.onComplete += () => GameStateManager.CurrentState = GameStateManager.GameState.WaitingForSwap;
        return seqFullCircle;
    }

    /// <summary>
    /// Ищет и собирает все матчи на поле
    /// </summary>
    private static Sequence ScoreMatches(out bool successScore)
    {
        foreach (var tile in Tiles)
        {
            var chip = tile?.Chip;
            if (chip == null || chip.IsScored)
            {
                continue;
            }
            var chips = MatchFinder.GetMaxMatchChips(chip);
            if (chips == null || chips.Count == 0)
            {
                continue;
            }
            Debug.Log($"Scored {chips.Count} chips!");
            foreach (var rowChip in chips)
            {
                // Если удалять фишки здесь, уберется только максимальный матч
                rowChip.IsScored = true;
            }
        }
        // Собирает все помеченные фишки.
        successScore = false;
        var seq = DOTween.Sequence();
        foreach (var tile in Tiles)
        {
            var chip = tile?.Chip;
            if (chip && chip.IsScored)
            {
                successScore = true;
                seq.Join(chip.Disappear());
            }
        }
        return seq;
    }

    private static Sequence FallAll()
    {
        var seqFallAll = DOTween.Sequence();
        do
        {
            seqFallAll
                .Append(FallAllOnce(true))
                .Append(FallAllOnce(false));
        } while (CanAnyChipFall());

        return seqFallAll;
    }

    private static bool CanAnyChipFall()
    {
        foreach (var tile in Tiles)
        {
            if (tile == null || tile.Chip == null)
            {
                continue;
            }

            if (tile.Chip.CanChipFallAnywhere())
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Один проход по полю, каждая клетка пытается упасть до конца вниз.
    /// Клетки с эмиттерами обрабатываются по несколько раз.
    /// </summary>
    /// <param name="verticalOnly"></param>
    /// <returns></returns>
    private static Sequence FallAllOnce(bool verticalOnly)
    {
        var seqFallAllChips = DOTween.Sequence();
        var fromOneEmitterInRow = 0;
        var chipWasTherePrevPass = false;
        
        for (var x = 0; x < BoardSize; x++)
        for (var y = BoardSize - 1; y >= 0; y--)
        {
            var tile = Tiles[y, x];

            if (tile == null)
            {
                continue;
            }
            
            // Если есть эмиттер, то создается фишка и падает.
            // Если его нет, фишка просто падает.
            if (tile.Emitter != null && tile.Chip == null)
            {
                chipWasTherePrevPass = false;
                y++;
                seqFallAllChips.Join(tile.Emitter.TrySpawnAndFall(fromOneEmitterInRow++ * Chip.MoveTime, verticalOnly));
                continue;
            }
            if (tile.Emitter != null && tile.Chip != null)
            {
                // Если в тайле с эмиттером осталась фишка с прошлого прохода
                // Значит ей некуда падать, поэтому переход к след. тайлу.
                if (chipWasTherePrevPass)
                {
                    chipWasTherePrevPass = false;
                    fromOneEmitterInRow = 0;
                    continue;
                }
                
                seqFallAllChips.Join(tile.Chip.FallAllWay(verticalOnly));
                
                chipWasTherePrevPass = true;
                y++;
                seqFallAllChips.Join(tile.Emitter.TrySpawnAndFall(fromOneEmitterInRow++ * Chip.MoveTime, verticalOnly));
                continue;
            }

            if (tile.Chip == null)
            {
                continue;;
            }
            
            seqFallAllChips.Join(tile.Chip.FallAllWay(verticalOnly));
            
            fromOneEmitterInRow = 0;
        }

        return seqFallAllChips;
    }
}
