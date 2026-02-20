using Godot;

public partial class HSliderScript : HSlider
{
    [Export]
    public string BigStepUpAction = "ui_page_up";

    [Export]
    public string BigStepDownAction = "ui_page_down";

    [Export]
    public float BigStepAmount = 10;

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (!IsVisibleInTree()) return;
        if (!HasFocus()) return;

        if (Input.IsActionJustReleased(BigStepUpAction))
        {
            Value += BigStepAmount;
            GetViewport().SetInputAsHandled();
        }
        else if (Input.IsActionJustReleased(BigStepDownAction))
        {
            Value -= BigStepAmount;
            GetViewport().SetInputAsHandled();
        }
    }
}
