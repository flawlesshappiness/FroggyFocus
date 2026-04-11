using Godot;
using System;

public partial class RaceController : SingletonController
{
    public static RaceController Instance => Singleton.Get<RaceController>();
    public override string Directory => "Race";

    public RaceSettings CurrentSettings { get; private set; }
    public RaceTrack CurrentTrack => CurrentSettings?.Track;

    public event Action OnCheckpoint;
    public event Action OnRaceStart;
    public event Action OnRaceEnd;

    public void StartRace(RaceSettings settings)
    {
        CurrentSettings = settings;
        settings.Track.OnCheckpoint += RaceTrack_Checkpoint;

        Player.SetAllLocks(nameof(RaceController), true);
        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1f,
            OnTransition = OnTransitionToStart
        });
    }

    private void OnTransitionToStart()
    {
        Player.SetAllLocks(nameof(RaceController), false);

        var start = CurrentSettings.Track.PlayerStart;
        Player.Instance.GlobalPosition = start.GlobalPosition;
        Player.Instance.Character.RotateToDirectionImmediate(start.Basis * Vector3.Forward);
        Player.Instance.ThirdPersonCamera.SetRotation(start.GlobalRotation);

        CurrentSettings.Track.StartRace();

        RaceView.Instance.Show();

        OnRaceStart?.Invoke();
    }

    public void EndRace()
    {
        Player.SetAllLocks(nameof(RaceController), true);

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 1f,
            OnTransition = OnTransitionToEnd
        });
    }

    private void OnTransitionToEnd()
    {
        RaceView.Instance.Hide();

        CurrentTrack.SetCheckpointsVisible(false);

        Player.SetAllLocks(nameof(RaceController), false);

        var end = CurrentSettings.Track.PlayerEnd;
        Player.Instance.GlobalPosition = end.GlobalPosition;
        Player.Instance.Character.RotateToDirectionImmediate(end.Basis * Vector3.Forward);
        Player.Instance.ThirdPersonCamera.SetRotation(end.GlobalRotation);

        OnRaceEnd?.Invoke();
    }

    private void RaceTrack_Checkpoint()
    {
        OnCheckpoint?.Invoke();
    }
}
