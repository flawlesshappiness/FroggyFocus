using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryContainer : ControlScript
{
    [Export]
    public bool ButtonsDisabled;

    [Export]
    public InventoryPreviewButton InventoryButtonTemplate;

    [Export]
    public Label EmptyLabel;

    [Export]
    public GridContainer GridContainer;

    [Export]
    public ScrollContainerScript ScrollContainer;

    public enum Mode { Press, Select }
    public Mode ButtonMode { get; set; } = Mode.Press;
    public List<InventoryCharacterData> Selection { get; private set; } = new();

    public event Action<InventoryCharacterData> OnButtonPressed;
    public event Action<InventoryCharacterData> OnButtonFocus;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public InventoryPreviewButton Button { get; set; }
        public FocusCharacterInfo Info { get; set; }
        public InventoryCharacterData Data { get; set; }
    }

    public void Clear()
    {
        maps.ForEach(x => x.Button.QueueFree());
        maps.Clear();
        Selection.Clear();
    }

    public void UpdateButtons(InventoryFilterOptions filter = null)
    {
        InventoryButtonTemplate.Hide();

        Clear();

        foreach (var data in Data.Game.Inventory.Characters)
        {
            if (!InventoryController.Instance.IsDataValid(data, filter)) continue;

            var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
            if (info == null) continue;

            var button = InventoryButtonTemplate.Duplicate() as InventoryPreviewButton;
            button.SetParent(InventoryButtonTemplate.GetParent());
            button.SetCharacter(info);
            button.Disabled = ButtonsDisabled;
            button.Show();

            var map = new ButtonMap
            {
                Button = button,
                Info = info,
                Data = data
            };

            button.Pressed += () => Button_Pressed(map);
            button.FocusEntered += () => Button_FocusEntered(map);

            maps.Add(map);
        }

        EmptyLabel.Visible = Data.Game.Inventory.Characters.Count == 0;
    }

    private void Button_Pressed(ButtonMap map)
    {
        ToggleSelected(map);
        OnButtonPressed?.Invoke(map.Data);
    }

    private void Button_FocusEntered(ButtonMap map)
    {
        OnButtonFocus?.Invoke(map.Data);
    }

    public Button GetFirstButton()
    {
        if (maps.Count == 0) return null;

        return maps.First().Button;
    }

    public void SetMode(Mode mode)
    {
        ButtonMode = mode;
        Selection.Clear();
        maps.ForEach(x => x.Button.SetChecked(false));
    }

    private void ToggleSelected(ButtonMap map)
    {
        if (ButtonMode != Mode.Select) return;

        if (map.Button.IsChecked)
        {
            Selection.Remove(map.Data);
        }
        else
        {
            Selection.Add(map.Data);
        }

        map.Button.SetChecked(!map.Button.IsChecked);
    }
}
