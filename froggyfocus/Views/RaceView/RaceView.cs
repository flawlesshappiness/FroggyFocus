using Godot;

public partial class RaceView : View
{
    public static RaceView Instance => Get<RaceView>();

    [Export]
    public Label CheckpointLabel;

    [Export]
    public Label CountdownLabel;

    [Export]
    public AudioStreamPlayer SfxCheckpoint;

    [Export]
    public AudioStreamPlayer SfxCountdown;

    [Export]
    public AudioStreamPlayer SfxGo;

    [Export]
    public AnimationPlayer Animation_Countdown;

    public override void _Ready()
    {
        base._Ready();
        RaceController.Instance.OnCheckpoint += Checkpoint;
        RaceController.Instance.OnCountdown += Countdown;
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateCheckpointLabel();
    }

    private void Checkpoint()
    {
        SfxCheckpoint.Play();
        UpdateCheckpointLabel();
    }

    public void UpdateCheckpointLabel()
    {
        var track = RaceController.Instance.CurrentTrack;
        var index = track?.CheckpointIndex ?? 0;
        var max = track?.CheckpointCount ?? 0;
        CheckpointLabel.Text = $"{index} / {max}";
    }

    private void Countdown(int i)
    {
        var is_go = i <= 0;
        CountdownLabel.Text = !is_go ? $"{i}" : "GO";

        var anim = !is_go ? "bounce" : "go";
        Animation_Countdown.Stop();
        Animation_Countdown.Play(anim);

        var sfx = is_go ? SfxGo : SfxCountdown;
        sfx.Play();
    }
}
