using Godot;

public partial class ButtonScript : Button
{
    [Export]
    public SoundInfo SfxFocusEnter;

    [Export]
    public SoundInfo SfxPressed;

    public override void _Ready()
    {
        base._Ready();
        MouseExited += Button_MouseExit;
        MouseEntered += Button_MouseEnter;
        Pressed += Button_Pressed;
        FocusEntered += Button_FocusEnter;
        FocusExited += Button_FocusExit;
    }

    protected virtual void Button_MouseEnter()
    {
        GrabFocus();
    }

    protected virtual void Button_MouseExit()
    {
    }

    protected virtual void Button_Pressed()
    {
        SfxPressed?.Play();
    }

    protected virtual void Button_FocusEnter()
    {
        SfxFocusEnter?.Play();
    }

    protected virtual void Button_FocusExit()
    {
    }
}
