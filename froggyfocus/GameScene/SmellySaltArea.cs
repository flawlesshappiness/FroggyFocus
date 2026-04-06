using Godot;
using System.Collections.Generic;

public partial class SmellySaltArea : Area3D, IInteractable
{
    private string DialogueSaltRock = "SALT_ROCK";
    private string DialogueSaltRockTake = "SALT_ROCK_TAKE";
    private string DialogueSaltRockLick = "SALT_ROCK_LICK";

    public override void _Ready()
    {
        base._Ready();
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnDialogueEnded -= DialogueEnded;
    }

    public void Interact()
    {
        DialogueController.Instance.StartDialogue(DialogueSaltRock);
    }

    private void DialogueEnded(string id)
    {
        if (id == DialogueSaltRock)
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

        DialogueController.Instance.StartDialogue(DialogueSaltRockTake);
    }

    private void LickSalt()
    {
        DialogueController.Instance.StartDialogue(DialogueSaltRockLick);
    }
}
