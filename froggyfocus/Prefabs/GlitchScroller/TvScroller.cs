using Godot;
using System.Collections;

public partial class TvScroller : Node2D
{
    [Export]
    public string SceneName;

    [Export]
    public string StartNode;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public TriggerArea2D PlayerArea;

    [Export]
    public Marker2D PlayerStartMarker;

    [Export]
    public bool FacingRight;

    public override void _Ready()
    {
        base._Ready();
        PlayerArea.OnNodeEntered += PlayerEntered;
    }

    private void PlayerEntered(Node2D node)
    {
        var player = node as SideScrollerController;
        player.Disable();
        AnimationPlayer.Play("flash");

        this.StartCoroutine(Cr, "travel");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(0.5f);
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 0.5f,
                OnTransition = OnTransition
            });
        }

        void OnTransition()
        {
            Data.Game.CurrentScene = SceneName;
            Data.Game.StartingNode = StartNode;
            Data.Game.Save();

            Scene.Goto(SceneName);
        }
    }

    public void SpawnPlayer(SideScrollerController player)
    {
        player.Disable();
        this.StartCoroutine(Cr, "spawn");
        IEnumerator Cr()
        {
            PlayerArea.SetCollidersEnabled(false);
            player.GlobalPosition = PlayerStartMarker.GlobalPosition;

            yield return new WaitForSeconds(1f);

            AnimationPlayer.Play("flash");
            player.SetFacingRight(FacingRight);
            player.SetState(SideScrollerController.State.Jump);
            player.Velocity = PlayerStartMarker.RightDirection() * 600f;

            yield return new WaitForSeconds(1f);

            PlayerArea.SetCollidersEnabled(true);
        }
    }
}
