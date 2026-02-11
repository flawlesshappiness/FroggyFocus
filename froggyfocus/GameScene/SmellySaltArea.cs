using Godot;
using System.Collections.Generic;

public partial class SmellySaltArea : Area3D, IInteractable
{
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
        DialogueController.Instance.StartDialogue("##SALT_ROCK_001##");
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##SALT_ROCK_001##")
        {
            StartOptions();
        }
    }

    private void StartOptions()
    {
        var options = new List<DialogueOptionsView.Option>();

        if (GameFlags.IsFlag(EldritchFlower.HasSaltsFlag, 0))
        {
            options.Add(new()
            {
                Text = "##SALT_ROCK_TAKE_OPTION##",
                Action = TakeSalt
            });
        }

        options.Add(new()
        {
            Text = "##DO_NOTHING##",
        });

        options.Add(new()
        {
            Text = "##SALT_ROCK_LICK_OPTION##",
            Action = LickSalt
        });

        DialogueOptionsView.Instance.ShowDialogueOptions(new()
        {
            Options = options
        });
    }

    private void TakeSalt()
    {
        GameFlags.SetFlag(EldritchFlower.HasSaltsFlag, 1);
        Data.Game.Save();

        DialogueController.Instance.StartDialogue("##SALT_ROCK_TAKE_001##");
    }

    private void LickSalt()
    {
        DialogueController.Instance.StartDialogue("##SALT_ROCK_LICK_001##");
    }
}
