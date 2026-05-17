using Godot;
using Godot.Collections;
using System.Linq;

public partial class EldritchTentacleWall : Area3D, IInteractable
{
    [Export]
    public CollisionShape3D InteractCollider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<EldritchEye> Eyes;

    [Export]
    public Array<EldritchTentacleObjective> Objectives;

    private const string DialogueEyes3 = "ELDRITCH_WALL_THREE";
    private const string DialogueEyes2 = "ELDRITCH_WALL_TWO";
    private const string DialogueEyes1 = "ELDRITCH_WALL_ONE";
    private const string DialogueComplete = "ELDRITCH_WALL_COMPLETE";

    private const string FlagComplete = "ELDRITCH_WALL_COMPLETE";

    private bool IsComplete => GameFlags.IsFlag(FlagComplete, 1);

    public override void _Ready()
    {
        base._Ready();

        for (int i = 0; i < Objectives.Count; i++)
        {
            var idx = i;
            var o = Objectives[idx];
            o.OnCompletedChanged += () => Objective_CompletedChanged(idx);

            if (o.IsCompleted)
            {
                Objective_CompletedChanged(idx);
            }
        }

        var anim = IsComplete ? "hidden" : "RESET";
        AnimationPlayer.Play(anim);

        InteractCollider.Disabled = IsComplete;

        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
        GameFlagsController.Instance.OnFlagChanged += GameFlag_Changed;
    }

    private void Objective_CompletedChanged(int i)
    {
        var objective = Objectives[i];
        var eye = Eyes[i];
        eye.SetOpen(!objective.IsCompleted);

        if (!objective.IsCompleted)
        {
            GameFlags.SetFlag(FlagComplete, 0);
        }
    }

    public void Interact()
    {
        var count_completed = Objectives.Count(x => x.IsCompleted);
        var is_completed = Objectives.All(x => x.IsCompleted);

        if (count_completed == 0)
        {
            DialogueController.Instance.StartDialogue(DialogueEyes3);
        }
        else if (count_completed == 1)
        {
            DialogueController.Instance.StartDialogue(DialogueEyes2);
        }
        else if (count_completed == 2)
        {
            DialogueController.Instance.StartDialogue(DialogueEyes1);
        }
        else if (is_completed)
        {
            DialogueController.Instance.StartDialogue(DialogueComplete);
        }
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialogueComplete)
        {
            GameFlags.SetFlag(FlagComplete, 1);
            Data.Game.Save();

            InteractCollider.Disabled = true;
        }
    }

    private void GameFlag_Changed(string id, int i)
    {
        if (id == FlagComplete)
        {
            if (i == 0)
            {
                InteractCollider.Disabled = false;
                AnimationPlayer.Play("RESET");
            }
            else if (i == 1)
            {
                InteractCollider.Disabled = true;
                AnimationPlayer.Play("hide");
            }
        }
    }
}
