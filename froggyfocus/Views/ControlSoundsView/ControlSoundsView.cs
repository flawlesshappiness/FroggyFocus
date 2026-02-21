using Godot;

public partial class ControlSoundsView : View
{
    [Export]
    public AudioStreamPlayer SfxFocusEntered;

    [Export]
    public AudioStreamPlayer SfxButtonPressed;

    [Export]
    public AudioStreamPlayer SfxSliderValueChanged;

    [Export]
    public AudioStreamPlayer SfxTabSelected;

    public override void _Ready()
    {
        base._Ready();
        ConnectExistingNodes();
        GetTree().NodeAdded += NodeAdded;
    }

    private void ConnectExistingNodes()
    {
        var children = GetTree().Root.GetChildren();
        foreach (var child in children)
        {
            NodeAdded(child);
        }
    }

    private void NodeAdded(Node node)
    {
        if (node is Button button)
        {
            button.FocusEntered += Control_FocusEntered;
            button.Pressed += Button_Pressed;
        }
        else if (node is Slider slider)
        {
            slider.FocusEntered += Control_FocusEntered;
            slider.ValueChanged += Slider_ValueChanged;
        }
        else if (node is TabContainer tab_container)
        {
            tab_container.GetTabBar().TabSelected += TabBar_TabSelected;
        }
    }

    private void Button_Pressed()
    {
        SfxButtonPressed.Play();
    }

    private void Control_FocusEntered()
    {
        SfxFocusEntered.Play();
    }

    private void Slider_ValueChanged(double value)
    {
        SfxSliderValueChanged.Play();
    }

    private void TabBar_TabSelected(long tab)
    {
        SfxTabSelected.Play();
    }
}
