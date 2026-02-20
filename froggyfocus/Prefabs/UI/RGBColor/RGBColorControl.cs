using Godot;
using Godot.Collections;
using System;

public partial class RGBColorControl : MarginContainer
{
    [Export]
    public Button FocusButton;

    [Export]
    public Array<Slider> Sliders;

    [Export]
    public Array<Label> Labels;

    public int R => (int)Sliders[0].Value;
    public int G => (int)Sliders[1].Value;
    public int B => (int)Sliders[2].Value;
    public Color Color => new Color(R / 255.0f, G / 255.0f, B / 255.0f);
    public bool IsFocused { get; private set; }

    public event Action<bool> OnFocusChanged;
    public event Action<Color> OnColorChanged;

    private bool is_loading;
    private bool is_focused_this_frame;
    private bool has_used_slider;

    public override void _Ready()
    {
        base._Ready();

        FocusButton.Pressed += FocusButton_Pressed;
        FocusButton.FocusMode = FocusModeEnum.All;

        for (int i = 0; i < Sliders.Count; i++)
        {
            var idx = i;
            var slider = Sliders[i];
            slider.ValueChanged += _ => Slider_ValueChanged(idx);
            slider.FocusMode = FocusModeEnum.None;
            slider.Editable = false;
        }
    }

    public override void _UnhandledInput(InputEvent e)
    {
        base._UnhandledInput(e);

        if (IsFocused && IsVisibleInTree() && !is_focused_this_frame)
        {
            if (Input.IsActionJustReleased("ui_cancel") || Input.IsActionJustReleased("ui_accept"))
            {
                SetFocused(false);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    public override void _Input(InputEvent e)
    {
        base._Input(e);

        if (IsFocused && IsVisibleInTree() && !is_focused_this_frame)
        {
            if (e is InputEventMouseButton button && button.ButtonIndex == MouseButton.Left && button.IsReleased())
            {
                if (has_used_slider)
                {
                    has_used_slider = false;
                }
                else if (!IsMouseInside())
                {
                    SetFocused(false);
                    GetViewport().SetInputAsHandled();
                }
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        is_focused_this_frame = false;
    }

    public void Load(Color color)
    {
        is_loading = true;
        var r = color.R * 255;
        var g = color.G * 255;
        var b = color.B * 255;
        Sliders[0].Value = r;
        Sliders[1].Value = g;
        Sliders[2].Value = b;
        Labels[0].Text = $"{r}";
        Labels[1].Text = $"{g}";
        Labels[2].Text = $"{b}";
        is_loading = false;
    }

    private void Slider_ValueChanged(int i)
    {
        if (is_loading) return;

        Labels[i].Text = $"{Sliders[i].Value}";
        ColorChanged();
        OnColorChanged?.Invoke(Color);
    }

    protected virtual void ColorChanged()
    {
        has_used_slider = true;
    }

    private void FocusButton_Pressed()
    {
        SetFocused(true);
    }

    private void SetFocused(bool focus)
    {
        IsFocused = focus;
        is_focused_this_frame = true;
        has_used_slider = false;

        FocusButton.FocusMode = focus ? FocusModeEnum.None : FocusModeEnum.All;
        FocusButton.Visible = !focus;

        foreach (var slider in Sliders)
        {
            slider.FocusMode = focus ? FocusModeEnum.All : FocusModeEnum.None;
            slider.Editable = focus;
        }

        Control control = focus ? Sliders[0] : FocusButton;
        control.GrabFocus();

        OnFocusChanged?.Invoke(focus);

        Data.Game.Save();
    }

    private bool IsMouseInside()
    {
        var rect = GetGlobalRect();
        return rect.HasPoint(GetGlobalMousePosition());
    }
}
