using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    public static int BoardSize = 9;

    public static Tile[,] Tiles = new Tile[BoardSize,BoardSize];


    public Sequence FallAll()
    {
        var seqFallAll = DOTween.Sequence();
        do
        {
            seqFallAll.Append(FallAllOnce());// FallAllOnce();
        } while (CanAnyChipFall());

        return seqFallAll;
    }

    private bool CanAnyChipFall()
    {
        foreach (var tile in Tiles)
        {
            if (tile?.Chip == null)
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
    
    private Sequence FallAllOnce()
    {
        var seqFallVertical = DOTween.Sequence(); // Хранит сиквенсы каждой фишки о падении вниз
        var fromOneEmitterInRow = 0;
        var chipWasTherePrevPass = false;
        
        // Только вертикальное падение
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
            if (tile.HasEmitter && tile.Chip == null)
            {
                var seqEmitterTile = tile.Emitter.TrySpawnChip(fromOneEmitterInRow++ * Chip.MoveTime)
                    .Append(tile.Chip.FallAllWayVertical());
                chipWasTherePrevPass = false;
                y++;
                seqFallVertical.Join(seqEmitterTile);
                continue;
            }
            if (tile.HasEmitter && tile.Chip != null)
            {
                // Если в тайле с эмиттером осталась фишка с прошлого прохода
                // Значит ей некуда падать, поэтому переход к след. тайлу.
                if (chipWasTherePrevPass)
                {
                    chipWasTherePrevPass = false;
                    continue;
                }
                seqFallVertical.Join(tile.Chip.FallAllWayVertical());
                chipWasTherePrevPass = true;
                // Создается фишка и падает вниз
                var seqEmitterTile = tile.Emitter.TrySpawnChip(fromOneEmitterInRow++ * Chip.MoveTime)
                    .Append(tile.Chip.FallAllWayVertical());
                y++;
                seqFallVertical.Join(seqEmitterTile);
                continue;
            }

            if (tile.Chip == null)
            {
                continue;;
            }
            seqFallVertical.Join(tile.Chip.FallAllWayVertical());
            
            fromOneEmitterInRow = 0;
        }

        fromOneEmitterInRow = 0;
        chipWasTherePrevPass = false;
        var seqFallAllWay = DOTween.Sequence();
        
        // И вертикальное и вбок.
        for (var x = 0; x < BoardSize; x++)
        for (var y = BoardSize - 1; y >= 0; y--)
        {
            var tile = Tiles[y, x];
            if (tile == null)
            {
                continue;
            }
            
            if (tile.HasEmitter && tile.Chip == null)
            {
                var seqEmitterTile = tile.Emitter.TrySpawnChip(fromOneEmitterInRow++ * Chip.MoveTime)
                    .Append(tile.Chip.FallAllWay());
                chipWasTherePrevPass = false;
                y++;
                seqFallAllWay.Join(seqEmitterTile);
                continue;
            }
            if (tile.HasEmitter && tile.Chip != null)
            {
                if (chipWasTherePrevPass)
                {
                    chipWasTherePrevPass = false;
                    continue;
                }
                seqFallAllWay.Join(tile.Chip.FallAllWay());
                chipWasTherePrevPass = true;
                // Создается фишка и падает вниз
                var seqEmitterTile = tile.Emitter.TrySpawnChip(fromOneEmitterInRow++ * Chip.MoveTime)
                    .Append(tile.Chip.FallAllWay());
                y++;
                seqFallAllWay.Join(seqEmitterTile);
                continue;
            }

            if (tile.Chip == null)
            {
                continue;
            }
            
            seqFallAllWay.Join(tile.Chip.FallAllWay());
            fromOneEmitterInRow = 0;
        }
        
        seqFallVertical.Append(seqFallAllWay);
        return seqFallVertical;
    }

    /// <summary>
    /// Ищет и собирает все матчи на поле
    /// </summary>
    public Sequence ScoreMatches(out bool successScore)
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
                // Анимация собирания здесь
                seq.Join(chip.Disappear());
            }
        }
        return seq;
    }
}
