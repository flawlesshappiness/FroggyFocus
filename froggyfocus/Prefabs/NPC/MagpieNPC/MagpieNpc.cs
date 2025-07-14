using FlawLizArt.Animation.StateMachine;
using Godot;
using Godot.Collections;
using System.Linq;

public partial class MagpieNpc : Area3D, IInteractable
{
    [Export]
    public FetchInfo FetchInfo;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    [Export]
    public Array<Pickup> Pickups;

    private bool active_dialogue;
    private bool begin_fetch;
    private bool end_fetch;
    private BoolParameter param_dialogue = new BoolParameter("dialogue", false);
    private TriggerParameter param_nod = new TriggerParameter("nod");

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
        DialogueController.Instance.OnNodeStarted += DialogueNodeStarted;

        InitializeAnimations();
        InitializePickups();
        InitializeFetch();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var idle_look = Animation.CreateAnimation("Armature|idle_look", false);
        var look = Animation.CreateAnimation("Armature|look", true);
        var look_tilt = Animation.CreateAnimation("Armature|look_tilt", false);
        var nod = Animation.CreateAnimation("Armature|nod", false);

        idle.AddVariation(idle_look, 0.25f);
        look.AddVariation(look_tilt, 0.25f);

        Animation.Connect(idle_look, look, param_dialogue.WhenTrue());
        Animation.Connect(idle, look, param_dialogue.WhenTrue());
        Animation.Connect(look, idle, param_dialogue.WhenFalse());
        Animation.Connect(look_tilt, idle, param_dialogue.WhenFalse());

        Animation.Connect(idle_look, idle);
        Animation.Connect(look_tilt, look);

        Animation.Connect(idle, nod, param_nod.WhenTriggered());
        Animation.Connect(idle_look, nod, param_nod.WhenTriggered());
        Animation.Connect(look, nod, param_nod.WhenTriggered());
        Animation.Connect(look_tilt, nod, param_nod.WhenTriggered());

        Animation.Connect(nod, idle);

        Animation.Start(idle.Node);
    }

    private void InitializePickups()
    {
        foreach (var pickup in Pickups)
        {
            pickup.SetPickupEnabled(false);
            pickup.OnPickup += Pickup_PickedUp;
        }
    }

    private void InitializeFetch()
    {
        Fetch.InitializeData(FetchInfo);
        var data = Fetch.GetOrCreateData(FetchInfo.Id);

        if (data.Started)
        {
            EnableFetchPickups();
        }
    }

    public void Interact()
    {
        if (Fetch.IsAvailable(FetchInfo.Id))
        {
            var data = Fetch.GetOrCreateData(FetchInfo.Id);
            if (data.Started)
            {
                if (data.Count > 0)
                {
                    DialogueController.Instance.AddVariable("{count}", $"{data.Count}");
                    StartDialogue("##MAGPIE_SWAMP_FETCH_COUNT##");
                }
                else
                {
                    end_fetch = true;
                    param_nod.Trigger();
                    StartDialogue("##MAGPIE_SWAMP_FETCH_COMPLETE_001##");
                }
            }
            else
            {
                begin_fetch = true;
                StartDialogue("##MAGPIE_SWAMP_FETCH_001##");
            }
        }
        else
        {
            StartDialogue("##MAGPIE_SWAMP_IDLE_001##");
        }
    }

    private void StartDialogue(string id)
    {
        active_dialogue = true;
        param_dialogue.Set(true);
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueEnded()
    {
        active_dialogue = false;
        param_dialogue.Set(false);

        if (begin_fetch)
        {
            begin_fetch = false;

            var data = Fetch.GetOrCreateData(FetchInfo.Id);
            data.Started = true;
            Data.Game.Save();

            EnableFetchPickups();
        }
        else if (end_fetch)
        {
            end_fetch = false;

            var data = Fetch.GetOrCreateData(FetchInfo.Id);
            Money.Add(data.MoneyReward);
            Fetch.ResetData(FetchInfo);
            Data.Game.Save();
        }
    }

    private void DialogueNodeStarted(string id)
    {
        if (!active_dialogue) return;

        SfxSpeak.Play();
    }

    private void EnableFetchPickups()
    {
        var data = Fetch.GetOrCreateData(FetchInfo.Id);
        var available_pickups = Pickups.ToList();

        for (int i = 0; i < data.Count; i++)
        {
            if (available_pickups.Count == 0) break;

            var pickup = available_pickups.PopRandom();
            pickup.SetPickupEnabled(true);
        }
    }

    private void Pickup_PickedUp()
    {
        var data = Fetch.GetOrCreateData(FetchInfo.Id);
        data.Count--;
        Data.Game.Save();
    }
}
