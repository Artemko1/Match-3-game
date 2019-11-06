using UnityEngine;

public class Tile : MonoBehaviour
{
    private const float EmitterOffset = TileManager.TileSize / 2;
    
    public Chip Chip;
    public Emitter Emitter;

    public int y, x;

    public Tile UpTile
    {
        get
        {
            if (y == 0)
            {
                return null;
            }
            return BoardManager.Tiles[y - 1, x];
        }
    }

    public Tile RightTile
    {
        get
        {
            if (x == BoardManager.BoardSize - 1)
            {
                return null;
            }
            return BoardManager.Tiles[y, x + 1];
        }
    }

    public Tile LeftTile
    {
        get
        {
            if (x == 0)
            {
                return null;
            }
            return BoardManager.Tiles[y, x - 1];
        }
    }

    public Tile DownTile
    {
        get
        {
            if (y == BoardManager.BoardSize - 1)
            {
                return null;
            }
            return BoardManager.Tiles[y + 1, x];
        }
    }

    public Tile DownRightTile
    {
        get
        {
            if (y == BoardManager.BoardSize - 1 || x == BoardManager.BoardSize - 1)
            {
                return null;
            }
            return BoardManager.Tiles[y + 1, x + 1];
        }
    }

    public Tile DownLeftTile
    {
        get
        {
            if (y == BoardManager.BoardSize - 1 || x == 0)
            {
                return null;
            }
            return BoardManager.Tiles[y + 1, x - 1];
        }
    }

    private GameObject EmitterGo;

    /// <summary>
    /// Перемещает фишку на тайл
    /// </summary>
    /// <param name="chip"></param>
    public void SetChip(Chip chip)
    {
        if (chip == null)
        {
            return;
        }
        if (Chip != null)
        {
            Debug.LogError("Фишка установлена в непустую клетку");
        }
        Chip = chip;
        Chip.transform.position = transform.position;
        Chip.Tile = this;
    }

    public void DestroyChip()
    {
        Destroy(Chip?.gameObject);
        Chip = null;
    }

    public void DestroyWithChildren()
    {
        DestroyChip();
        RemoveEmitter();
        Destroy(gameObject);
    }

    public void CreateEmitter()
    {
        var emitterPos = transform.position;
        emitterPos.y += EmitterOffset;
        EmitterGo = Instantiate(TileManager.EmitterPrefab);
        EmitterGo.transform.position = emitterPos;
        Emitter = new Emitter(this);
    }

    private void RemoveEmitter()
    {
        Emitter = null;
        Destroy(EmitterGo);
    }
}
