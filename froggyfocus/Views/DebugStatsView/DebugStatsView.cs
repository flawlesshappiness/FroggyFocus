using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public partial class DebugStatsView : View
{
    [Export]
    public Button CloseButton;

    [Export]
    public DebugStatsSlider StatsSliderTemplate;

    private List<DebugStatsSlider> sliders = new();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        CloseButton.Pressed += CloseClicked;
    }

    protected override void OnShow()
    {
        base.OnShow();
        ClearSliders();
        CreateSliders();
    }

    private void RegisterDebugActions()
    {
        var category = nameof(DebugStatsView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = v => { v.Close(); Show(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Hide",
            Action = v => { v.Close(); Close(); }
        });
    }

    private void ClearSliders()
    {
        foreach (var slider in sliders)
        {
            slider.QueueFree();
        }

        sliders.Clear();
    }

    private void CreateSliders()
    {
        var types = Enum.GetValues(typeof(StatsType)).Cast<StatsType>().ToList();
        var parent = StatsSliderTemplate.GetParent();

        foreach (var type in types)
        {
            var info = StatsController.Instance.GetInfo(type);
            var data = StatsController.Instance.GetOrCreateData(type);
            var values = StatsController.Instance.GetStatsValueArray(type);
            var slider = StatsSliderTemplate.Duplicate() as DebugStatsSlider;
            slider.SetParent(parent);
            slider.Show();

            slider.NameLabel.Text = type.ToString();
            slider.ValueLabel.Text = values[data.Level].ToString();
            slider.ValueSlider.MinValue = 0;
            slider.ValueSlider.MaxValue = values.Length - 1;
            slider.ValueSlider.Value = data.Level;
            slider.ValueSlider.ValueChanged += d =>
            {
                var level = (int)d;
                data.Level = level;
                slider.ValueLabel.Text = values[level].ToString();
            };

            sliders.Add(slider);
        }

        StatsSliderTemplate.Hide();
    }

    private void CloseClicked()
    {
        Close();
    }

    private void Close()
    {
        Data.Game.Save();
        Hide();
    }
}
