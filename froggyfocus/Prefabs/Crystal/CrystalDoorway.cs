using Godot;
using System.Collections.Generic;

public partial class CrystalDoorway : Area3D, IInteractable
{
    [Export]
    public bool IsEntrance;

    [Export]
    public AudioStreamPlayer3D SfxLocked;

    private string SceneName => IsEntrance ? nameof(CrystalScene) : nameof(CaveScene);
    private string StartNode => IsEntrance ? "" : "CrystalStart";
    private bool IsOpen => !IsEntrance || GameFlags.IsFlag(IsOpenFlag, 1);

    private const string TryCodeFlag = "CRYSTAL_DOOR_TRY_CODE";
    public const string HasCodeFlag = "CRYSTAL_DOOR_HAS_CODE";
    public const string IsOpenFlag = "CRYSTAL_DOOR_OPEN";

    private readonly List<string> TryCodeDialogueNodes = new()
    {
        "##CRYSTAL_DOOR_ATTEMPT_001##",
        "##CRYSTAL_DOOR_ATTEMPT_002##",
        "##CRYSTAL_DOOR_ATTEMPT_003##",
        "##CRYSTAL_DOOR_ATTEMPT_004##",
        "##CRYSTAL_DOOR_ATTEMPT_005##",
    };

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
    }

    public void Interact()
    {
        if (IsOpen)
        {
            ChangeScene();
        }
        else
        {
            SfxLocked.Play();
            DialogueController.Instance.StartDialogue("##CRYSTAL_DOOR_LOCKED##");
        }
    }

    private void ChangeScene()
    {
        Data.Game.StartingNode = StartNode;
        Data.Game.CurrentScene = SceneName;
        Data.Game.Save();

        SfxLocked.Play();

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1f,
            OnTransition = () =>
            {
                if (IsEntrance)
                {
                    var scene = Scene.Goto<CrystalScene>();
                    scene.StartIntro();
                }
                else
                {
                    Scene.Goto(SceneName);
                }
            }
        });
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##CRYSTAL_DOOR_LOCKED##")
        {
            if (GameFlags.IsFlag(HasCodeFlag, 1))
            {
                ShowCorrectCodeOptions();
            }
            else
            {
                ShowIncorrectCodeOptions();
            }
        }
        else if (id == "##CRYSTAL_DOOR_UNLOCK##")
        {
            ChangeScene();
        }
    }

    private void TryIncorrectCode()
    {
        if (GameFlags.GetFlag(TryCodeFlag) >= 4)
        {
            DialogueController.Instance.StartDialogue("##CRYSTAL_DOOR_RANDOM_RESULT##");
        }
        else
        {
            GameFlags.IncrementFlag(TryCodeFlag);
            Data.Game.Save();

            DialogueController.Instance.StartDialogue("##CRYSTAL_DOOR_INCORRECT##");
        }
    }

    private void TryCorrectCode()
    {
        GameFlags.SetFlag(IsOpenFlag, 1);
        Data.Game.Save();

        DialogueController.Instance.StartDialogue("##CRYSTAL_DOOR_UNLOCK##");
    }

    private void ShowIncorrectCodeOptions()
    {
        var try_id = TryCodeDialogueNodes.GetClamped(GameFlags.GetFlag(TryCodeFlag));

        DialogueOptionsView.Instance.ShowDialogueOptions(new()
        {
            Options = new()
                {
                    new()
                    {
                        Text = try_id,
                        Action = TryIncorrectCode
                    },

                    new()
                    {
                        Text = "##DO_NOTHING##",
                    }
                }
        });
    }

    private void ShowCorrectCodeOptions()
    {
        DialogueOptionsView.Instance.ShowDialogueOptions(new()
        {
            Options = new()
                {
                    new()
                    {
                        Text = "##CRYSTAL_DOOR_ATTEMPT_REAL_CODE##",
                        Action = TryCorrectCode
                    },

                    new()
                    {
                        Text = "##DO_NOTHING##",
                    }
                }
        });
    }
}
