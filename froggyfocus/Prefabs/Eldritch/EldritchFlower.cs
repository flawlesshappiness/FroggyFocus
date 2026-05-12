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

    private const string DialogueSnoring = "ELDRITCH_FLOWER_SNORING";
    private const string DialogueSalt = "ELDRITCH_FLOWER_USE_SALTS";
    private const string DialogueNothing = "DO_NOTHING";
    private const string DialogueRumble = "ELDRITCH_FLOWER_RUMBLE";

    public override void _Ready()
    {
        base._Ready();
        Initialize();
        DialogueController.Instance.OnEntryEnded += DialogueNodeEnded;
        GameFlagsController.Instance.OnFlagChanged += GameFlag_Changed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnEntryEnded -= DialogueNodeEnded;
        GameFlagsController.Instance.OnFlagChanged -= GameFlag_Changed;
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
            Collider.Disabled = false;
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
            DialogueController.Instance.StartDialogue(DialogueSnoring);
        }
    }

    private void DisableInteractive()
    {
        Collider.Disabled = true;
    }

    private void DialogueNodeEnded(string id)
    {
        if (id == DialogueSnoring)
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
                Text = DialogueSalt,
                Action = UseSalts
            });
        }

        options.Add(new()
        {
            Text = DialogueNothing,
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
            DialogueController.Instance.StartDialogue(DialogueRumble);
        }
    }

    private void GameFlag_Changed(string id, int i)
    {
        if (id == IsAwakeFlag)
        {
            if (i == 0)
            {
                Initialize();
            }
        }
    }
}
