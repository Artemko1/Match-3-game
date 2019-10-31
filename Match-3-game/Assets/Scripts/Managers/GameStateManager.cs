using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        WaitingForSwap,
        Swapping,
        Falling
    }

    public static GameState CurrentState = GameState.WaitingForSwap;
    
    private static BoardManager BoardManager;

    public static Sequence ScoreAndFallUntilSettle()
    {
        bool successScore;
        var seqFullCircle = DOTween.Sequence();
        do
        {
            seqFullCircle
                .Append(BoardManager.ScoreMatches(out successScore))
                .Append(BoardManager.FallAll());

        } while (successScore);
        
        seqFullCircle.onPlay += () => CurrentState = GameState.Falling;
        seqFullCircle.onComplete += () => CurrentState = GameState.WaitingForSwap;
        return seqFullCircle;
    }

    
    private void Awake()
    {
        BoardManager = FindObjectOfType<BoardManager>();
    }
}