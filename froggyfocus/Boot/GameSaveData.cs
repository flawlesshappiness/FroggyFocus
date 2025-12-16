public partial class GameSaveData
{
    public bool FirstTimeBoot { get; set; } = true;
    public string CurrentScene { get; set; } = nameof(LetterScene);
    public string StartingNode { get; set; }
}
