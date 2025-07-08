using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryContainer : ControlScript
{
    [Export]
    public bool ShowValue;

    [Export]
    public GridContainer GridContainer;

    [Export]
    public InventoryPreviewButton InventoryButtonTemplate;

    [Export]
    public Label EmptyLabel;

    public event Action<FocusCharacterInfo> OnButtonPressed;
    public event Action<FocusCharacterInfo> OnButtonFocus;

    private ButtonMap selected_map;
    private List<ButtonMap> maps = new();

    public class FilterOptions
    {
        public List<FocusCharacterInfo> ValidCharacters { get; set; }
        public List<FocusCharacterInfo> ExcludedCharacters { get; set; }
        public List<InventoryCharacterData> ExcludedDatas { get; set; }
    }

    private class ButtonMap
    {
        public Button Button { get; set; }
        public FocusCharacterInfo Info { get; set; }
        public InventoryCharacterData Data { get; set; }
    }

    public void Clear()
    {
        selected_map = null;
        maps.ForEach(x => x.Button.QueueFree());
        maps.Clear();
    }

    public void UpdateButtons(FilterOptions filter = null)
    {
        InventoryButtonTemplate.Hide();

        Clear();

        foreach (var data in Data.Game.Inventory.Characters)
        {
            if (!IsDataValid(data, filter)) continue;

            var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
            if (info == null) continue;

            var button = InventoryButtonTemplate.Duplicate() as InventoryPreviewButton;
            button.SetParent(InventoryButtonTemplate.GetParent());
            button.SetCharacter(info);
            button.ValueContainer.Visible = ShowValue;
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

        UpdateGridContainer();
    }

    private bool IsDataValid(InventoryCharacterData data, FilterOptions options = null)
    {
        if (options == null) return true;

        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);

        if (options.ExcludedDatas?.Contains(data) ?? false) return false;
        if (!options.ValidCharacters?.Contains(info) ?? false) return false;
        if (options.ExcludedCharacters?.Contains(info) ?? false) return false;

        return true;
    }

    private void Button_Pressed(ButtonMap map)
    {
        selected_map = map;
        OnButtonPressed?.Invoke(map.Info);
    }

    private void Button_FocusEntered(ButtonMap map)
    {
        OnButtonFocus?.Invoke(map.Info);
    }

    public void PressFirstButton()
    {
        if (maps.Count == 0) return;

        var map = maps.First();
        Button_Pressed(map);
    }

    public Button GetFirstButton()
    {
        if (maps.Count == 0) return null;

        return maps.First().Button;
    }

    public InventoryCharacterData GetSelectedData()
    {
        return selected_map?.Data;
    }

    private void UpdateGridContainer()
    {
        var v_separation = "v_separation";

        if (ShowValue)
        {
            GridContainer.AddThemeConstantOverride(v_separation, 30);
        }
        else
        {
            GridContainer.RemoveThemeConstantOverride(v_separation);
        }
    }
}
