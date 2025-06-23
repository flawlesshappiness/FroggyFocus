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

    public event Action<FocusCharacterInfo> OnButtonPressed;

    private ButtonMap selected_map;
    private List<ButtonMap> maps = new();

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

    public void UpdateButtons()
    {
        InventoryButtonTemplate.Hide();

        Clear();

        foreach (var data in Data.Game.Inventory.Characters)
        {
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

            maps.Add(map);
        }

        UpdateGridContainer();
    }

    private void Button_Pressed(ButtonMap map)
    {
        selected_map = map;
        OnButtonPressed?.Invoke(map.Info);
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
