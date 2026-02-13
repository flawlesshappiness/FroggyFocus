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

    private bool HasRemote => GameFlags.IsFlag(HasRemoteFlag, 1);
    private bool IsOn => GameFlags.IsFlag(TvOnFlag, 1);
    private string DebugId => nameof(TvTravel) + GetInstanceId();

    private bool initialized;

    public override void _Ready()
    {
        base._Ready();
        SetOn(IsOn);

        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
        GameFlagsController.Instance.OnFlagChanged += GameFlagChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
        GameFlagsController.Instance.OnFlagChanged -= GameFlagChanged;
    }

    private void GameFlagChanged(string id, int i)
    {
        if (id == TvOnFlag)
        {
            SetOn(i == 1);
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##TV_OFF##")
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
            DialogueController.Instance.StartDialogue("##TV_OFF##");
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
        DialogueController.Instance.StartDialogue("##TV_LICK_001##");
    }

    private void RemoteOption()
    {
        DialogueController.Instance.StartDialogue("##TV_REMOTE_001##");
        GameFlags.SetFlag(TvOnFlag, 1);
    }
}
