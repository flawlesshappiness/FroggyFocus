using Godot;
using Godot.Collections;

public partial class TabContainerScript : TabContainer
{
    [Export]
    public int IconMaxWidth;

    [Export]
    public Array<Texture2D> Icons;

    public override void _Ready()
    {
        base._Ready();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < Icons.Count; i++)
        {
            var icon = Icons[i];
            if (icon != null)
            {
                SetTabIcon(i, icon);
                SetTabIconMaxWidth(i, IconMaxWidth);
                SetTabTitle(i, string.Empty);
            }
        }
    }
}
