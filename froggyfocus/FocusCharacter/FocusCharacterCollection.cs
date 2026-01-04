using Godot;

[GlobalClass]
public partial class FocusCharacterCollection : ResourceCollection<FocusCharacterInfo>
{
    [Export]
    public FocusCharacterInfo BugOfData;

    [Export]
    public FocusCharacterInfo BugOfLife;

    [Export]
    public FocusCharacterInfo BugOfLove;
}
