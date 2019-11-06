using DG.Tweening;
using UnityEngine;

public static class ChipSelector
{
    private static Chip SelectedChip;
    
    public static void OnChipEnter(Chip chip)
    {
        if (GameStateManager.CurrentState != GameStateManager.GameState.WaitingForSwap)
        {
            return;
        }

        if (SelectedChip == null)
        {
            return;
        }

        if (CanSwap(SelectedChip, chip))
        {
            TrySwapWithSelected(chip);
            DeselectChip(SelectedChip);
        }
    }
    
    public static void OnChipClick(Chip clickedChip)
    {
        if (GameStateManager.CurrentState != GameStateManager.GameState.WaitingForSwap)
        {
            return;
        }
        
        if (SelectedChip == null)
        {
            SelectChip(clickedChip);
        }
        else if (SelectedChip == clickedChip)
        {
            DeselectChip(clickedChip);
        }
        else if (CanSwap(SelectedChip, clickedChip))
        {
            TrySwapWithSelected(clickedChip);
            DeselectChip(SelectedChip);
        }
        else
        {
            DeselectChip(SelectedChip);
            SelectChip(clickedChip);
        }
    }
    
    private static Sequence SwapChips(Chip first, Chip second)
    {
        
        var secondTile = second.Tile;
        var firstTile = first.Tile;
        var seqSwap = DOTween.Sequence()
            .Append(first.Move(secondTile))
            .Join(second.Move(firstTile));
        seqSwap.onPlay += () => GameStateManager.CurrentState = GameStateManager.GameState.Swapping;
        seqSwap.onComplete += () => GameStateManager.CurrentState = GameStateManager.GameState.WaitingForSwap;
        first.Tile.Chip = first;
        return seqSwap;
    }

    private static void TrySwapWithSelected(Chip secondChip)
    {
        var seq = DOTween.Sequence()
            .Append(SwapChips(SelectedChip, secondChip));
        if (MatchFinder.IsInMatch(SelectedChip) || MatchFinder.IsInMatch(secondChip))
        {
            seq.Append(BoardManager.ScoreAndFallUntilSettle());
        }
        else
        {
            // Свапает фишки обратно
            seq.Append(SwapChips(SelectedChip, secondChip));
        }
    }
    
    private static bool CanSwap(Chip first, Chip second)
    {
        return first.LeftChip == second || first.RightChip == second || first.UpChip == second || first.DownChip == second;
    }

    private static void SelectChip(Chip chip)
    {
        SelectedChip = chip;
        chip.transform.DORotate(chip.transform.rotation.eulerAngles + new Vector3(0, 0, -30), 0.1f);
    }
    private static void DeselectChip(Chip chip)
    {
        SelectedChip = null;
        chip.transform.DORotate(Vector3.zero, 0.1f);
    }
}