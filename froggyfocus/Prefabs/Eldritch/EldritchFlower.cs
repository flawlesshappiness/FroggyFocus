using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class EldritchFlower : Area3D, IInteractable
{
    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer Animation;

    public bool IsAwake => GameFlags.IsFlag(IsAwakeFlag, 1);

    public const string IsAwakeFlag = "ELDRITCH_FLOWER_AWAKE";
    public const string HasSaltsFlag = "ELDRITCH_FLOWER_HAS_SALTS";

    public override void _Ready()
    {
        base._Ready();
        Initialize();
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
    }

    private void Initialize()
    {
        if (IsAwake)
        {
            DisableInteractive();
            Animation.Play("inactive");
        }
        else
        {
            Animation.Play("breathe");
        }
    }

    public void Interact()
    {
        if (IsAwake)
        {

        }
        else
        {
            DialogueController.Instance.StartDialogue("##ELDRITCH_FLOWER_SNORING##");
        }
    }

    private void DisableInteractive()
    {
        Collider.Disabled = true;
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == "##ELDRITCH_FLOWER_SNORING##")
        {
            ShowOptions();
        }
    }

    private void ShowOptions()
    {
        var options = new List<DialogueOptionsView.Option>();

        if (GameFlags.IsFlag(HasSaltsFlag, 1))
        {
            options.Add(new()
            {
                Text = "##ELDRITCH_FLOWER_USE_SALTS##",
                Action = UseSalts
            });
        }

        options.Add(new()
        {
            Text = "##DO_NOTHING##",
        });

        DialogueOptionsView.Instance.ShowDialogueOptions(new()
        {
            Options = options
        });
    }

    private void UseSalts()
    {
        DisableInteractive();

        this.StartCoroutine(Cr, "awaken");
        IEnumerator Cr()
        {
            yield return Animation.PlayAndWaitForAnimation("coughing");
            Animation.Play("hide");
            GameFlags.SetFlag(IsAwakeFlag, 1);
            DialogueController.Instance.StartDialogue("##ELDRITCH_FLOWER_RUMBLE##");
        }
    }
}
