using Godot;
using System.Collections;

public partial class TransitionScene : GameScene
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Camera3D Camera;

    [Export]
    public AudioStreamPlayer Music;

    public override void _Ready()
    {
        base._Ready();
        Camera.Current = true;
        SetLocks(true);
        AnimationPlayer.Play("sail");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        SetLocks(false);
    }

    public Coroutine AnimateTransition()
    {
        return this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Music.Play();
            yield return new WaitForSeconds(5f);
        }
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(TransitionScene);
        Player.SetAllLocks(id, locked);
        PauseView.ToggleLock.SetLock(id, locked);
    }
}
