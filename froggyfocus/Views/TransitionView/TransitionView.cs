using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class TransitionView : View
{
    public static TransitionView Instance => Get<TransitionView>();

    [Export]
    public AnimationPlayer AnimationPlayer_Color;

    [Export]
    public AnimationPlayer AnimationPlayer_Shapes;

    [Export]
    public AnimationPlayer AnimationPlayer_Lilypads;

    [Export]
    public Array<Control> ColorControls;

    public void StartTransition(TransitionSettings settings)
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Show();
            Player.SetAllLocks(nameof(TransitionView), true);
            var animation = GetAnimationPlayer(settings.Type);
            animation.SpeedScale = 1f / settings.Duration;
            SetColor(settings.Color);
            yield return animation.PlayAndWaitForAnimation("show");
            settings.OnTransition?.Invoke();
            Player.SetAllLocks(nameof(TransitionView), false);
            yield return animation.PlayAndWaitForAnimation("hide");
            Hide();
        }
    }

    private void SetColor(Color color)
    {
        ColorControls.ForEach(x => x.Modulate = color);
    }

    private AnimationPlayer GetAnimationPlayer(TransitionType type)
    {
        return type switch
        {
            TransitionType.Color => AnimationPlayer_Color,
            TransitionType.Circles => AnimationPlayer_Shapes,
            TransitionType.Lilypads => AnimationPlayer_Lilypads,
            _ => AnimationPlayer_Color
        };
    }
}

public class TransitionSettings
{
    public TransitionType Type { get; set; } = TransitionType.Color;
    public float Duration { get; set; } = 1f;
    public Color Color { get; set; } = Colors.Black;
    public Action OnTransition { get; set; }
}

public enum TransitionType
{
    Color, Circles, Lilypads
}