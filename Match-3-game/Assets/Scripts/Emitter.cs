using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    public Tile Tile;
    
    private ChipManager ChipManager;

    public Sequence TrySpawnChip(float spawnDelay)
    {
        if (Tile == null)
        {
            return null;
        }
        if (Tile.Chip == null)
        {
            Tile.SetChip(ChipManager.CreateRandomChip());
            Tile.Chip.transform.position += Vector3.up * TileManager.TileSize;
            var chipSprite = Tile.Chip.GetComponent<SpriteRenderer>();
            var tmp = chipSprite.color;
            tmp.a = 0f;
            chipSprite.color = tmp;
            var seq = DOTween.Sequence().Append(Tile.Chip.Appear(spawnDelay));
            return seq;
        }
        return null;
        
    }

    public Sequence TrySpawnChipAndFallVertical()
    {
        if (Tile?.Chip == null)
        {
            Tile?.SetChip(ChipManager.CreateRandomChip());
            var seq = DOTween.Sequence();//.AppendInterval(delay);
            return seq;
        }

        return null;
    }
    
    private void Awake()
    {
        ChipManager = GameObject.Find("ChipManager").GetComponent<ChipManager>();
        
    }
}
