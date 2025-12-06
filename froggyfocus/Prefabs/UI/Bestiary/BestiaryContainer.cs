using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BestiaryContainer : ControlScript
{
    [Export]
    public InventoryPreviewButton InventoryButtonTemplate;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public InventoryPreviewButton Button { get; set; }
        public FocusCharacterInfo Info { get; set; }
    }

    protected override void OnHide()
    {
        base.OnHide();
        InventoryButtonTemplate.Hide();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        var infos = GetCharacters();
        foreach (var info in infos)
        {
            var button = CreateButton();
            var map = new ButtonMap
            {
                Button = button,
                Info = info,
            };

            button.SetCharacter(info);
            button.Pressed += () => Button_Pressed(map);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        foreach (var map in maps)
        {
            var stats = StatsController.Instance.GetOrCreateCharacterData(map.Info.ResourcePath);
            var visible = stats.CountCaught > 0;
            map.Button.SetObscured(!visible);
        }
    }

    private InventoryPreviewButton CreateButton()
    {
        var button = InventoryButtonTemplate.Duplicate() as InventoryPreviewButton;
        button.SetParent(InventoryButtonTemplate.GetParent());
        button.Show();
        return button;
    }

    public Button GetFirstButton()
    {
        if (maps.Count == 0) return null;
        return maps.First().Button;
    }

    private IEnumerable<FocusCharacterInfo> GetCharacters()
    {
        return FocusCharacterController.Instance.Collection.Resources
            .Where(x => x.Name == x.Variation && !x.ExcludeFromBestiary);
    }

    private void Button_Pressed(ButtonMap map)
    {

    }
}
