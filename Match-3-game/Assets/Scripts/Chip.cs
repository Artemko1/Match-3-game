using UnityEngine;
using DG.Tweening;

public class Chip : MonoBehaviour
{
    public const float MoveTime = 0.2f;
    private const float AppearTime = 0.2f;
    
    public Tile Tile;
    public int Type;
    public bool IsPreset;
    public bool IsScored;


    public Chip UpChip => Tile.UpTile?.Chip;
    public Chip RightChip => Tile.RightTile?.Chip;
    public Chip LeftChip => Tile.LeftTile?.Chip;
    public Chip DownChip => Tile.DownTile?.Chip;


    public Tween Move(Tile tileToMove)
    {
        Tile.Chip = null;
        Tile = tileToMove;
        Tile.Chip = this;
        return transform.DOMove(tileToMove.transform.position, MoveTime).SetEase(Ease.Linear);
    }


    public Sequence Appear(float appearDelay)
    {
        var seqAppear = DOTween.Sequence().AppendInterval(appearDelay);
        seqAppear.Append(GetComponent<SpriteRenderer>().DOFade(1f, AppearTime));
        seqAppear.Join(transform.DOMove(Tile.transform.position, AppearTime).SetEase(Ease.Linear));
        return seqAppear;
    }

    public Tween Disappear()
    {
        Tile.Chip = null;
        Tween tween = transform.DOScale(0.1f, 0.25f);
        tween.onComplete += () => Destroy(gameObject);

        return tween;
    }

    public Sequence FallAllWay(bool verticalOnly)
    {
        var seq = DOTween.Sequence();
        var cantMove = false;
        do
        {
            if (CanChipFallThere(Tile.DownTile))
            {
                seq.Append(Move(Tile.DownTile));
            }
            else if (!verticalOnly && CanChipFallThere(Tile.DownLeftTile))
            {
                seq.Append(Move(Tile.DownLeftTile));
            }
            else if (!verticalOnly && CanChipFallThere(Tile.DownRightTile))
            {
                seq.Append(Move(Tile.DownRightTile));
            }
            else
            {
                cantMove = true;
            }
        } while (!cantMove);

        return seq;
    }

    public bool CanChipFallAnywhere()
    {
        if (Tile.DownTile != null && Tile.DownTile.Chip == null)
        {
            return true;
        }
        if (Tile.DownLeftTile != null && Tile.DownLeftTile.Chip == null)
        {
            return true;
        }
        if (Tile.DownRightTile != null && Tile.DownRightTile.Chip == null)
        {
            return true;
        }
        return false;
    }

    private bool CanChipFallThere(Tile tileToFall)
    {
        return tileToFall != null && tileToFall.Chip == null;
    }

    private void OnMouseDown()
    {
        ChipSelector.OnChipClick(this);
    }

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
        {
            ChipSelector.OnChipEnter(this);
        }
    }
}
