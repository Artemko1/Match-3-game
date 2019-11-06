public static class GameStateManager
{
    public enum GameState
    {
        WaitingForSwap,
        Swapping,
        Falling
    }

    public static GameState CurrentState = GameState.WaitingForSwap;
}