using Godot;

public partial class AppearancePreviewButton : ButtonScript
{
    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    public bool IsLocked { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = ItemSubViewport.GetTexture();

        FocusEntered += Button_FocusEntered;
        FocusExited += Button_FocusExited;
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        TextureRect.Modulate = locked ? Colors.Black.SetA(0.5f) : Colors.White;
    }

    public void SetAppearance(AppearanceInfo info)
    {
        var attachment = ItemSubViewport.SetPrefab<AppearanceAttachment>(info.Prefab);
        attachment.SetDefaultColors();
        TextureRect.Show();
    }

    private void Button_FocusEntered()
    {
        ItemSubViewport.SetAnimationSpin();
    }

    private void Button_FocusExited()
    {
        ItemSubViewport.SetAnimationIdle();
    }
}
