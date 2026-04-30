using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RaceInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public RaceGhostInfo GhostEasy;

    [Export]
    public RaceGhostInfo GhostHard;

    [Export]
    public string DialogueEasy;

    [Export]
    public string DialogueHard;

    [Export]
    public string DialogueRace;

    [Export]
    public string DialogueWinEasy;

    [Export]
    public string DialogueWinHard;

    [Export]
    public string DialogueLose;

    [Export]
    public Array<ItemType> RewardItemTypes;
}
