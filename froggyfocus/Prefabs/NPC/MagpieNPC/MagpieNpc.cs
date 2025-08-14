using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class MagpieNpc : CharacterNpc
{
    [Export]
    public HandInInfo HandInInfo;

    private TriggerParameter param_nod = new TriggerParameter("nod");

    public override void _Ready()
    {
        base._Ready();

        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;

        HandIn.InitializeData(HandInInfo);
    }

    protected override void InitializeAnimations()
    {
        base.InitializeAnimations();

        var idle_look = Animation.CreateAnimation("Armature|idle_look", false);
        var look_tilt = Animation.CreateAnimation("Armature|look_tilt", false);
        var nod = Animation.CreateAnimation("Armature|nod", false);

        IdleState.AddVariation(idle_look, 0.25f);
        DialogueState.AddVariation(look_tilt, 0.25f);

        Animation.Connect(idle_look, DialogueState, param_dialogue.WhenTrue());
        Animation.Connect(IdleState, DialogueState, param_dialogue.WhenTrue());
        Animation.Connect(DialogueState, IdleState, param_dialogue.WhenFalse());
        Animation.Connect(look_tilt, IdleState, param_dialogue.WhenFalse());

        Animation.Connect(idle_look, IdleState);
        Animation.Connect(look_tilt, DialogueState);

        Animation.Connect(IdleState, nod, param_nod.WhenTriggered());
        Animation.Connect(idle_look, nod, param_nod.WhenTriggered());
        Animation.Connect(DialogueState, nod, param_nod.WhenTriggered());
        Animation.Connect(look_tilt, nod, param_nod.WhenTriggered());

        Animation.Connect(nod, IdleState);
    }

    public override void Interact()
    {
        if (HandIn.IsAvailable(HandInInfo.Id))
        {
            StartDialogue("##MAGPIE_SWAMP_REQUEST_001##");
        }
        else
        {
            StartDialogue("##MAGPIE_SWAMP_IDLE_001##");
        }
    }

    private void DialogueNodeEnded(string id)
    {
        if (!HasActiveDialogue) return;

        if (id == "##MAGPIE_SWAMP_REQUEST_002##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
            Data.Game.Save();

            StartDialogue("##MAGPIE_SWAMP_REQUEST_COMPLETE_001##");
        }
    }
}
