using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class DebugContentList : ControlScript
{
    [Export]
    public Label Label;

    [Export]
    public ScrollContainer ScrollContainer;

    private List<Node> elements = new();

    public override void _Ready()
    {
        base._Ready();
        Label.Visible = false;
    }

    protected override void OnShow()
    {
        base.OnShow();
        ScrollDown();
    }

    public void Clear()
    {
        elements.ToList().ForEach(e => e.QueueFree());
        elements.Clear();
    }

    public Label AddText(string text)
    {
        var label = Label.Duplicate() as Label;
        label.SetParent(Label.GetParent());
        label.Visible = true;
        label.Text = text;

        elements.Add(label);

        return label;
    }

    public Label AddLog(LogMessage log)
    {
        var label = AddText(log.GetLogMessage());
        var color = log.Type == LogType.Trace ? Colors.Gray
                : log.Type == LogType.Error ? Colors.Red
                : log.Type == LogType.Exception ? Colors.DarkRed
                : Colors.White;
        label.AddThemeColorOverride("font_color", color);
        return label;
    }

    public void ScrollDown()
    {
        ScrollContainer.ScrollVertical = int.MaxValue;
    }
}
