using Godot;
using System.Collections.Generic;

public partial class TvTravel : Area3D, IInteractable
{
    [Export]
    public bool IsEntrance;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public const string HasRemoteFlag = "HAS_TV_REMOTE";
    public const string TvOnFlag = "IS_TV_ON";

    private const string DialogueTvOff = "TV_OFF";
    private const string DialogueTvLick = "TV_LICK";
    private const string DialogueTvRemote = "TV_REMOTE";

    private bool HasRemote => GameFlags.IsFlag(HasRemoteFlag, 1);
    private bool IsOn => GameFlags.IsFlag(TvOnFlag, 1);
    private string DebugId => nameof(TvTravel) + GetInstanceId();

    private bool initialized;

    public override void _Ready()
    {
        base._Ready();
        SetOn(IsOn);

        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
        GameFlagsController.Instance.OnFlagChanged += GameFlagChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= DialogueEnded;
        GameFlagsController.Instance.OnFlagChanged -= GameFlagChanged;
    }

    private void GameFlagChanged(string id, int i)
    {
        if (id == TvOnFlag)
        {
            SetOn(i == 1);
        }
    }

    private void DialogueEnded(string id)
    {
        if (id == DialogueTvOff)
        {
            ShowOptions();
        }
    }

    public void Interact()
    {
        if (IsOn)
        {
            ShowOptions();
        }
        else
        {
            DialogueController.Instance.StartDialogue(DialogueTvOff);
        }
    }

    private void SetOn(bool on)
    {
        var anim = on ? "on" : "off";
        AnimationPlayer.Play(anim);
    }

    private void Travel()
    {
        Data.Game.CurrentScene = IsEntrance ? nameof(GlitchScene) : nameof(FactoryScene);
        Data.Game.StartingNode = IsEntrance ? string.Empty : "TvStart";
        Data.Game.Save();

        GlitchTransitionView.Instance.IsGoingUp = !IsEntrance;
        GlitchTransitionView.Instance.StartTransition();
    }

    private void ShowOptions()
    {
        var options = new List<DialogueOptionsView.Option>();

        if (IsOn)
        {
            options.Add(new()
            {
                Text = "##TV_ENTER_OPTION##",
                Action = Travel
            });
        }
        else if (HasRemote)
        {
            options.Add(new()
            {
                Text = "##TV_REMOTE_OPTION##",
                Action = RemoteOption
            });
        }

        options.Add(new()
        {
            Text = "##DO_NOTHING##",
        });

        options.Add(new()
        {
            Text = "##TV_LICK_OPTION##",
            Action = LickOption
        });

        DialogueOptionsView.Instance.ShowDialogueOptions(new()
        {
            Options = options
        });
    }

    private void LickOption()
    {
        DialogueController.Instance.StartDialogue(DialogueTvLick);
    }

    private void RemoteOption()
    {
        DialogueController.Instance.StartDialogue(DialogueTvRemote);
        GameFlags.SetFlag(TvOnFlag, 1);
    }
}
