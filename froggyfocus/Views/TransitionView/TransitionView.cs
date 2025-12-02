using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class TransitionView : View
{
    public static TransitionView Instance => Get<TransitionView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<Control> ColorControls;

    public void StartTransition(TransitionSettings settings)
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Show();
            Player.SetAllLocks(nameof(TransitionView), true);
            AnimationPlayer.SpeedScale = 1f / settings.Duration;
            SetColor(settings.Color);
            yield return AnimationPlayer.PlayAndWaitForAnimation(settings.AnimationIn);
            settings.OnTransition?.Invoke();
            Player.SetAllLocks(nameof(TransitionView), false);
            yield return AnimationPlayer.PlayAndWaitForAnimation(settings.AnimationOut);
            Hide();
        }
    }

    private void SetColor(Color color)
    {
        ColorControls.ForEach(x => x.Modulate = color);
    }
}

public class TransitionSettings
{
    public TransitionType Type { get; set; } = TransitionType.Color;
    public float Duration { get; set; } = 1f;
    public Color Color { get; set; } = Colors.Black;
    public Action OnTransition { get; set; }

    public string AnimationIn => $"{Type.ToString().ToLower()}_in";
    public string AnimationOut => $"{Type.ToString().ToLower()}_out";
}

public enum TransitionType
{
    Color, Circles
}