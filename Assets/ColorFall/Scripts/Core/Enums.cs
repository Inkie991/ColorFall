namespace ColorFall.Core
{
    public enum Sound
    {
        ChangeColor,
        ButtonIn,
        ButtonOut,
        Lose,
        Win,
        DropCollect,
        WrongColor,
        Bounce,
        CoinCollect
    }
    
    public enum GamingColor
    {
        Red,
        Green,
        Blue
    }
    
    public enum ManagerStatus
    {
        Shutdown,
        Initializing,
        Started
    }

    public enum GenerationMode
    {
        Linear,
        Puzzle
    }

    public enum MaterialType
    {
        Default,
        Transparent,
        Sprite
    }

    public enum CollectedDrop
    {
        Correct,
        Incorrect
    }
}