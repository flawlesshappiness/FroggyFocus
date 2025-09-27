using Godot;

public partial class AppearanceTabScrollContainer : ScrollContainer
{
    [Export]
    public AppearanceContainer TopContainer;

    public override void _Ready()
    {
        base._Ready();
        TopContainer.TopButtonFocusEntered += TopButtonFocusEntered;
    }

    private void TopButtonFocusEntered()
    {
        ScrollToTop();
    }

    private void ScrollToTop()
    {
        ScrollVertical = 0;
    }
}
