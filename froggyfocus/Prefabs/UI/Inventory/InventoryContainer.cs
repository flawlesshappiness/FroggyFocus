using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class InventoryContainer : ControlScript
{
    [Export]
    public int RowCount;

    [Export]
    public bool ButtonsDisabled;

    [Export]
    public InventoryPreviewButton InventoryButtonTemplate;

    [Export]
    public Label EmptyLabel;

    [Export]
    public Label InvalidLabel;

    [Export]
    public Label PageLabel;

    [Export]
    public GridContainer GridContainer;

    [Export]
    public Button PreviousPageButton;

    [Export]
    public Button NextPageButton;

    public enum Mode { Press, Select }
    public Mode ButtonMode { get; set; } = Mode.Press;
    public List<InventoryCharacterData> Selection { get; private set; } = new();
    public int Page { get; private set; }
    public int PageCount { get; private set; }
    public int ButtonCount => RowCount * GridContainer.Columns;

    public event Action<InventoryCharacterData> OnButtonPressed;
    public event Action<InventoryCharacterData> OnButtonFocus;

    private List<InventoryPreviewButton> buttons = new();
    private List<DataMap> maps = new();
    private List<DataMap> filtered_maps = new();

    private class DataMap
    {
        public int Index { get; set; }
        public FocusCharacterInfo Info { get; set; }
        public InventoryCharacterData Data { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        InitializeButtons();

        PreviousPageButton.Pressed += PreviousPageButton_Pressed;
        NextPageButton.Pressed += NextPageButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();
        SetPage(0);
    }

    public void Clear()
    {
        Selection.Clear();
    }

    public void Update(InventoryFilterOptions filter = null)
    {
        InitializeDataMaps();
        filtered_maps = maps.Where(x => InventoryController.Instance.IsDataValid(x.Data, filter)).ToList();
        EmptyLabel.Visible = maps.Count == 0;
        InvalidLabel.Visible = !EmptyLabel.Visible && filtered_maps.Count == 0;
        PageCount = Mathf.Max(1, Mathf.CeilToInt(filtered_maps.Count / (float)ButtonCount));
        SetPage(Page);
    }

    private void InitializeButtons()
    {
        buttons.Clear();
        InventoryButtonTemplate.Hide();

        for (int i = 0; i < ButtonCount; i++)
        {
            var index = i;
            var button = InventoryButtonTemplate.Duplicate() as InventoryPreviewButton;
            button.SetParent(InventoryButtonTemplate.GetParent());
            button.Hide();
            button.Disabled = ButtonsDisabled;
            buttons.Add(button);

            button.Pressed += () => Button_Pressed(index);
            button.FocusEntered += () => Button_FocusEntered(index);
        }
    }

    private void InitializeDataMaps()
    {
        maps.Clear();

        var datas = Data.Game.Inventory.Characters;
        for (int i = 0; i < datas.Count; i++)
        {
            var data = datas[i];
            var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
            if (info == null) continue;

            var index = i;
            var map = new DataMap
            {
                Index = index,
                Info = info,
                Data = data
            };

            maps.Add(map);
        }
    }

    private DataMap GetMap(int button_index)
    {
        var map_index = ButtonCount * Page + button_index;
        return maps[map_index];
    }

    private void SetPage(int index)
    {
        Page = Mathf.PosMod(index, PageCount);
        PageLabel.Text = $"{Page + 1} / {PageCount}";
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        var start = Page * ButtonCount;
        var end = (Page + 1) * ButtonCount;
        var visible_maps = filtered_maps.Take(new System.Range(start, end)).ToList();
        for (int i = 0; i < buttons.Count; i++)
        {
            var map = i >= visible_maps.Count ? null : visible_maps[i];
            var button = buttons[i];
            button.Visible = map != null;

            if (map != null)
            {
                button.SetCharacter(map.Info);
                button.SetChecked(Selection.Contains(map.Data));
            }
        }
    }

    private void PreviousPageButton_Pressed()
    {
        SetPage(Page - 1);
    }

    private void NextPageButton_Pressed()
    {
        SetPage(Page + 1);
    }

    private void Button_Pressed(int index)
    {
        var button = buttons[index];
        var map = GetMap(index);
        ToggleSelected(button, map);
        OnButtonPressed?.Invoke(map.Data);
    }

    private void Button_FocusEntered(int index)
    {
        var map = GetMap(index);
        OnButtonFocus?.Invoke(map.Data);
    }

    public Button GetFirstButton()
    {
        if (maps.Count == 0) return null;

        return buttons.First();
    }

    public void SetMode(Mode mode)
    {
        ButtonMode = mode;
        Selection.Clear();
    }

    private void ToggleSelected(InventoryPreviewButton button, DataMap map)
    {
        if (ButtonMode != Mode.Select) return;

        var is_selected = Selection.Contains(map.Data);
        button.SetChecked(!is_selected);

        if (is_selected)
        {
            Selection.Remove(map.Data);
        }
        else
        {
            Selection.Add(map.Data);
        }
    }
}
