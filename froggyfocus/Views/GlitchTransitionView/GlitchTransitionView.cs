using Godot;
using System.Collections;

public partial class GlitchTransitionView : View
{
    public static GlitchTransitionView Instance => Get<GlitchTransitionView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public AudioStreamPlayer SfxBoom;

    public bool IsGoingUp { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Frog.SetFalling();
    }

    public void LoadScene()
    {
        Scene.Goto(Data.Game.CurrentScene);
    }

    public void StartTransition()
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            SetLocks(true);
            Show();
            PlayBoomSfx();

            var anim = IsGoingUp ? "transition_up" : "transition_down";
            yield return AnimationPlayer.PlayAndWaitForAnimation(anim);

            Hide();
            SetLocks(false);
        }
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(GlitchTransitionView);

        Player.SetAllLocks(id, locked);
        PauseView.ToggleLock.SetLock(id, locked);
    }

    public void PlayBoomSfx()
    {
        SfxBoom.Play();
    }
}
