using DG.Tweening;
using UnityEngine;

public class Emitter
{
    private readonly Tile Tile;

    public Emitter(Tile tile)
    {
        Tile = tile;
    }

    public Sequence TrySpawnAndFall(float spawnDelay, bool verticalOnly)
    {
        var seqSpawnAndFall = TrySpawnChip(spawnDelay)
            .Append(Tile.Chip.FallAllWay(verticalOnly));
        
        return seqSpawnAndFall;
    }

    private Sequence TrySpawnChip(float spawnDelay)
    {
        if (Tile == null) return null;

        if (Tile.Chip != null) return null;
        
        Tile.SetChip(ChipManager.CreateRandomChip());
        Tile.Chip.transform.position += Vector3.up * TileManager.TileSize;
        var chipSprite = Tile.Chip.GetComponent<SpriteRenderer>();
        var tmp = chipSprite.color;
        tmp.a = 0f;
        chipSprite.color = tmp;
        var seqSpawn = DOTween.Sequence().Append(Tile.Chip.Appear(spawnDelay));
        return seqSpawn;

    }
}
