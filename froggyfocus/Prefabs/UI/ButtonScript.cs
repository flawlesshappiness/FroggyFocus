using Godot;

public partial class ButtonScript : Button
{
    [Export]
    public SoundInfo SfxMouseEnter;

    [Export]
    public SoundInfo SfxMouseExit;

    [Export]
    public SoundInfo SfxPressed;

    public override void _Ready()
    {
        base._Ready();
        MouseExited += OnMouseExit;
        MouseEntered += OnMouseEnter;
        Pressed += OnPressed;
    }

    private void OnMouseEnter()
    {
        SfxMouseEnter?.Play();
    }

    private void OnMouseExit()
    {
        SfxMouseExit?.Play();
    }

    private void OnPressed()
    {
        SfxPressed?.Play();
    }
}
