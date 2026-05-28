using Godot;
using Godot.Collections;

public partial class FirewallDoorNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<CollisionShape3D> Colliders;

    public bool IsCompleted => HandInInfo.Data.ClaimCount >= 3;
    public int ClaimCount => HandInInfo.Data.ClaimCount;

    public const string DialogueIntro = "FIREWALL_INTRO";
    public const string DialogueLiftQuarantine = "FIREWALL_LIFT_QUARANTINE";
    public const string DialogueAnalyzingA = "FIREWALL_ANALYZING_A";
    public const string DialogueAnalyzingB = "FIREWALL_ANALYZING_B";
    public const string DialogueAnalyzingC = "FIREWALL_ANALYZING_C";

    public override void _Ready()
    {
        base._Ready();
        HandInController.Instance.OnHandInClaimed += HandIn_Claimed;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;

        AnimationPlayer.Play(IsCompleted ? "opened" : "closed");
        UpdateColliders();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandIn_Claimed;
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
    }

    public void Interact()
    {
        if (IsCompleted)
        {
            UpdateColliders();
        }
        else
        {
            DialogueController.Instance.StartDialogue(DialogueIntro);
        }
    }

    private void HandIn_Claimed(string id)
    {
        if (HandInInfo.Id != id) return;

        if (IsCompleted)
        {
            DialogueController.Instance.StartDialogue(DialogueAnalyzingC);
        }
        else if (ClaimCount == 1)
        {
            DialogueController.Instance.StartDialogue(DialogueAnalyzingA);
        }
        else if (ClaimCount == 2)
        {
            DialogueController.Instance.StartDialogue(DialogueAnalyzingB);
        }
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueIntro)
        {
            DialogueOptionsView.Instance.ShowDialogueOptions(new DialogueOptionsView.Settings
            {
                Options = new()
                {
                    new DialogueOptionsView.Option
                    {
                        Text = "##LIFT_QUARANTINE##",
                        Action = LiftQuarantineOption_Pressed,
                    },

                    new DialogueOptionsView.Option
                    {
                        Text = "##CANCEL##",
                    },
                }
            });
        }
        else if (id == DialogueLiftQuarantine || id == DialogueAnalyzingA || id == DialogueAnalyzingB)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
        else if (id == DialogueAnalyzingC)
        {
            UpdateColliders();
            AnimationPlayer.Play("open");
        }
    }

    private void LiftQuarantineOption_Pressed()
    {
        DialogueController.Instance.StartDialogue(DialogueLiftQuarantine);
    }

    private void UpdateColliders()
    {
        var disabled = IsCompleted;
        foreach (var collider in Colliders)
        {
            collider.SetDeferred("disabled", disabled);
        }
    }
}
