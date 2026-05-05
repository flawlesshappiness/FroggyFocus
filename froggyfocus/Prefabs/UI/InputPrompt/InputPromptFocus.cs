using Godot;

public partial class InputPromptFocus : Control
{
    [Export]
    public InputPromptTexture Texture;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public enum AnimationType
    {
        None,
        Tapping,
        LongPress,
    }

    public void SetInputAction(string action)
    {
        Texture.UpdateIcon(action);
    }

    public void SetAnimation(AnimationType type)
    {
        var animation = type switch
        {
            AnimationType.Tapping => "tapping",
            AnimationType.LongPress => "long_press",
            _ => "idle"
        };

        SetAnimation(animation);
    }

    private void SetAnimation(string animation)
    {
        AnimationPlayer.Play(animation);
    }
}
