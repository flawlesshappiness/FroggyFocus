using Godot;
using System.Collections;

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
    public AudioStreamPlayer BgmRace;

    [Export]
    public AnimationPlayer Animation_Countdown;

    public override void _Ready()
    {
        base._Ready();
        RaceController.Instance.OnCheckpoint += Checkpoint;
        RaceController.Instance.OnCountdown += Countdown;
        RaceController.Instance.OnCountdownStarted += RaceCountdown_Started;
        RaceController.Instance.OnTransitionToEnd += Race_TransitionToEnd;
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

    private void RaceCountdown_Started()
    {
        BgmRace.VolumeLinear = 0.9f;
        BgmRace.Play();
    }

    private void Race_TransitionToEnd()
    {
        this.StartCoroutine(Cr, "stop_bgm");
        IEnumerator Cr()
        {
            yield return BgmRace.FadeOut(1f);
            BgmRace.Stop();
        }
    }
}
