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

    [Export]
    public Node3D MatrixLabelParent;

    [Export]
    public PackedScene MatrixLabelPrefab;

    public bool IsGoingUp { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Frog.SetFalling();
        InitializeMatrixLabels();
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
            Frog.LoadAppearance();
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
    }

    public void PlayBoomSfx()
    {
        SfxBoom.Play();
    }

    private void InitializeMatrixLabels()
    {
        var rng = new RandomNumberGenerator();
        var count = 20;
        var extent = 1f;
        var radius_range = new Vector2(1, 3);
        var scale_range = new Vector2(0.005f, 0.02f);

        for (int i = 0; i < count; i++)
        {
            var radius = radius_range.Range(rng.Randf());
            var t_radius = (radius - radius_range.X) / (radius_range.Y - radius_range.X);
            var scale = scale_range.Range(t_radius);

            var label = MatrixLabelPrefab.Instantiate<Node3D>();
            label.SetParent(MatrixLabelParent);

            var x = rng.RandfRange(-extent, extent);
            var z = rng.RandfRange(-extent, extent);
            label.Position = new Vector3(x, 0, z).Normalized() * radius;

            label.Scale = Vector3.One * scale;
        }
    }
}
