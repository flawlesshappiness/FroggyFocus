using Godot;
using Godot.Collections;
using System;

public partial class RaceTrack : Node3D
{
    [Export]
    public Marker3D PlayerStart;

    [Export]
    public Marker3D PlayerEnd;

    [Export]
    public Array<RaceCheckpoint> Checkpoints;

    public event Action OnCheckpoint;

    public bool RaceCompleted { get; private set; }
    public bool RaceStarted { get; private set; }
    public int CheckpointIndex { get; private set; }
    public int CheckpointCount => Checkpoints.Count;

    public override void _Ready()
    {
        base._Ready();
        InitializeCheckpoints();
    }

    private void InitializeCheckpoints()
    {
        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.OnPlayerEntered += () => Checkpoint_PlayerEntered(checkpoint);
        }

        SetCheckpointsVisible(false);
    }

    private void Checkpoint_PlayerEntered(RaceCheckpoint checkpoint)
    {
        if (!RaceStarted) return;
        if (CheckpointIndex >= CheckpointCount) return;

        var expected = Checkpoints[CheckpointIndex];
        if (checkpoint != expected) return;

        CheckpointIndex++;
        checkpoint.AnimateHide();

        OnCheckpoint?.Invoke();

        if (CheckpointIndex >= Checkpoints.Count)
        {
            RaceCompleted = true;
            EndRace();
        }
        else
        {
            UpdateNextCheckpoint();
        }
    }

    private void UpdateNextCheckpoint()
    {
        var checkpoint = Checkpoints[CheckpointIndex];
        var is_finish = CheckpointIndex == Checkpoints.Count - 1;
        checkpoint.AnimateShow();
        checkpoint.SetFinish(is_finish);
    }

    public void StartRace()
    {
        RaceStarted = true;
        CheckpointIndex = 0;

        SetCheckpointsVisible(true);
        UpdateNextCheckpoint();
    }

    public void EndRace()
    {
        RaceStarted = false;
        RaceController.Instance.EndRace();
    }

    public void SetCheckpointsVisible(bool visible)
    {
        foreach (var checkpoint in Checkpoints)
        {
            checkpoint.Visible = visible;
            checkpoint.AnimateHideImmediate();
        }
    }
}
