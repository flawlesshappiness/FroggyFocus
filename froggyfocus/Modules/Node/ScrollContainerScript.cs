using Godot;
using System.Linq;

public partial class ScrollContainerScript : ScrollContainer
{
    [Export]
    public bool UpdateOnReady;

    [Export]
    public bool UpdateOnChildrenChanged;

    private bool children_changed;

    public override void _Ready()
    {
        base._Ready();
        if (UpdateOnReady)
        {
            ScrollChildrenChanged();
        }

        ChildEnteredTree += Child_EnteredTree;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (UpdateOnChildrenChanged && children_changed)
        {
            children_changed = false;
            ScrollChildrenChanged();
        }
    }

    private void Child_EnteredTree(Node node)
    {
        children_changed = true;
    }

    public void ScrollChildrenChanged()
    {
        var child = GetChild(0);

        if (child is MarginContainer margin)
        {
            child = margin.GetChild(0);
        }

        if (child is VBoxContainer vbox)
        {
            var first = vbox.GetChild(0) as Control;
            var last = vbox.GetChild(GetChildCount() - 1) as Control;
            first.FocusEntered += this.ScrollVerticalToTop;
            last.FocusEntered += this.ScrollVerticalToBottom;
        }
        else if (child is HBoxContainer hbox)
        {
            var first = hbox.GetChild(0) as Control;
            var last = hbox.GetChild(GetChildCount() - 1) as Control;
            first.FocusEntered += this.ScrollHorizontalToTop;
            last.FocusEntered += this.ScrollHorizontalToBottom;
        }
        else if (child is GridContainer grid)
        {
            var children = grid.GetChildren();
            var v_firsts = children.Take(grid.Columns).Select(x => x as Control);
            var v_lasts = children.Reverse<Node>().Take(grid.Columns).Select(x => x as Control);
            var h_firsts = children.Where((x, i) => i % grid.Columns == 0).Select(x => x as Control);
            var h_lasts = children.Where((x, i) => (i + 1) % grid.Columns == 0).Select(x => x as Control);

            v_firsts.ForEach(x => x.FocusEntered += this.ScrollVerticalToTop);
            v_lasts.ForEach(x => x.FocusEntered += this.ScrollVerticalToBottom);
            h_firsts.ForEach(x => x.FocusEntered += this.ScrollHorizontalToTop);
            h_lasts.ForEach(x => x.FocusEntered += this.ScrollHorizontalToBottom);
        }
    }
}
