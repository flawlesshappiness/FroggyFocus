using Godot;

public partial class InventoryPreviewButton : ButtonScript
{
    [Export]
    public TextureRect TextureRect;

    [Export]
    public ItemSubViewport ItemSubViewport;

    [Export]
    public PackedScene HiddenPreviewPrefab;

    [Export]
    public TextureRect BackgroundShadow;

    [Export]
    public TextureRect CheckIcon;

    public bool IsChecked => CheckIcon.Visible;

    public override void _Ready()
    {
        base._Ready();
        TextureRect.Texture = ItemSubViewport.GetTexture();
        ItemSubViewport.SetCameraInventory();

        FocusEntered += Button_FocusEntered;
        FocusExited += Button_FocusExited;
    }

    public void Clear()
    {
        ItemSubViewport.Clear();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        ItemSubViewport.SetCharacter(info);
    }

    public void SetHiddenPreview()
    {
        ItemSubViewport.SetPrefab(HiddenPreviewPrefab);
    }

    public void SetObscured(bool obscured)
    {
        var color = obscured ? Colors.Black.SetA(0.5f) : Colors.White;
        TextureRect.Modulate = color;
        BackgroundShadow.Visible = !obscured;
    }

    public void SetChecked(bool is_checked)
    {
        CheckIcon.Visible = is_checked;
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
