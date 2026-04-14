using Godot;
using System;
using System.Collections;

public partial class RaceController : SingletonController
{
    public static RaceController Instance => Singleton.Get<RaceController>();
    public override string Directory => "Race";

    public bool IsWin { get; private set; }
    public RaceSettings CurrentSettings { get; private set; }
    public RaceGhost CurrentGhost { get; private set; }
    public RaceTrack CurrentTrack => CurrentSettings?.Track;

    public event Action OnCheckpoint;
    public event Action OnRaceStart;
    public event Action<RaceResult> OnRaceEnd;
    public event Action<int> OnCountdown;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "RACE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Win race",
            Action = WinRace
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Lose race",
            Action = LoseRace
        });

        void WinRace(DebugView v)
        {
            if (CurrentGhost != null)
            {
                CurrentGhost.IsFinished = false;
            }

            EndRace();
            v.Close();
        }

        void LoseRace(DebugView v)
        {
            if (CurrentGhost != null)
            {
                CurrentGhost.IsFinished = true;
            }

            EndRace();
            v.Close();
        }
    }

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
        var is_ghost = RaceGhostController.Instance.RecordGhostEnabled;
        var start = is_ghost ? CurrentSettings.Track.GhostStart : CurrentSettings.Track.PlayerStart;
        Player.Instance.GlobalPosition = start.GlobalPosition;
        Player.Instance.Character.RotateToDirectionImmediate(start.Basis * Vector3.Forward);
        Player.Instance.ThirdPersonCamera.SetRotation(start.GlobalRotation);

        RaceView.Instance.Show();

        CurrentSettings.Track.StartRace();

        InitializeGhost();
        CountdownToStart();
    }

    private void InitializeGhost()
    {
        if (RaceGhostController.Instance.RecordGhostEnabled) return;
        if (string.IsNullOrEmpty(CurrentTrack.Id)) return;

        var start = CurrentSettings.Track.GhostStart;
        var ghost = RaceGhostController.Instance.CreateGhost();
        ghost.GlobalPosition = start.GlobalPosition;
        ghost.SetTargetPosition(start.GlobalPosition);
        ghost.GlobalRotation = start.GlobalRotation;
        ghost.SetTargetRotation(start.GlobalRotation);
        ghost.LoadData(CurrentTrack.Id);
        CurrentGhost = ghost;
    }

    private void ClearGhost()
    {
        if (CurrentGhost == null) return;
        CurrentGhost.QueueFree();
        CurrentGhost = null;
    }

    private void CountdownToStart()
    {
        this.StartCoroutine(Cr, "countdown");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(1f);

            var count = 3;
            for (int i = count; i > 0; i--)
            {
                OnCountdown?.Invoke(i);
                yield return new WaitForSeconds(1f);
            }

            RaceGhostController.Instance.StartRecordingGhost();
            Player.SetAllLocks(nameof(RaceController), false);

            CurrentGhost?.PlayGhost();

            OnCountdown?.Invoke(-1);
            OnRaceStart?.Invoke();
        }
    }

    public void EndRace()
    {
        IsWin = !CurrentGhost?.IsFinished ?? true;

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
        RaceGhostController.Instance.EndRecordingGhost();
        RaceView.Instance.Hide();

        CurrentTrack.SetCheckpointsVisible(false);

        ClearGhost();

        Player.SetAllLocks(nameof(RaceController), false);

        var end = CurrentSettings.Track.PlayerEnd;
        Player.Instance.GlobalPosition = end.GlobalPosition;
        Player.Instance.Character.RotateToDirectionImmediate(end.Basis * Vector3.Forward);
        Player.Instance.ThirdPersonCamera.SetRotation(end.GlobalRotation);

        var result = new RaceResult
        {
            IsWin = IsWin
        };

        OnRaceEnd?.Invoke(result);
    }

    private void RaceTrack_Checkpoint()
    {
        OnCheckpoint?.Invoke();
    }
}
