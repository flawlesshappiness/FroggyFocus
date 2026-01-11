using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class HandInContainer : MarginContainer
{
    [Export]
    public Control ControlsContainer;

    [Export]
    public Control TimerContainer;

    [Export]
    public Button BackButton;

    [Export]
    public Button ClaimButton;

    [Export]
    public Button PinButton;

    [Export]
    public Label TimerLabel;

    [Export]
    public Label NameLabel;

    [Export]
    public RichTextLabel LocationHintLabel;

    [Export]
    public RewardUnlockBar RewardUnlockBar;

    [Export]
    public Array<InventoryPreviewButton> RequestButtons;

    [Export]
    public Array<RewardPreview> RewardPreviews;

    private List<ButtonMap> maps = new();

    public event Action OnClaim;

    public HandInData CurrentData { get; private set; }
    public HandInInfo CurrentInfo { get; private set; }
    public bool CurrentClaimed { get; private set; }
    public bool HasItemUnlock => CurrentInfo?.HasItemUnlock ?? false;
    public bool IsMaxClaim => CurrentData?.ClaimedCount >= CurrentInfo?.ClaimCountToUnlock;
    private List<InventoryCharacterData> Submissions => maps.Select(x => x.Submission).Where(x => x != null).ToList();

    private class ButtonMap
    {
        public int Index { get; set; }
        public InventoryPreviewButton Button { get; set; }
        public InventoryCharacterData Submission { get; set; }
        public void Clear()
        {
            Button.Clear();
            Submission = null;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        ClaimButton.Pressed += ClaimButton_Pressed;
        PinButton.Pressed += PinButton_Pressed;

        InitializeRequestButtons();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_TimerLabel();
    }

    private void InitializeRequestButtons()
    {
        for (int i = 0; i < RequestButtons.Count; i++)
        {
            var idx = i;
            var button = RequestButtons[i];
            var map = new ButtonMap
            {
                Index = idx,
                Button = button,
            };

            button.FocusEntered += () => RequestButton_FocusEntered(map);
            maps.Add(map);
        }
    }

    private void Clear()
    {
        maps.ForEach(x => x.Clear());
        ClaimButton.Disabled = true;
        CurrentData = null;
        RewardUnlockBar.Clear();
        CurrentClaimed = false;
    }

    public void Load(HandInData data)
    {
        Clear();

        CurrentData = data;
        CurrentInfo = HandInController.Instance.GetInfo(data.Id);

        var is_available = HandIn.IsAvailable(data.Id);
        ControlsContainer.Visible = is_available;
        TimerContainer.Visible = !is_available;
        PinButton.Visible = is_available;

        CurrentClaimed = false;

        LoadControls(data);
    }

    private void LoadControls(HandInData data)
    {
        RequestButtons.ForEach(x => x.Hide());

        for (int i = 0; i < data.RequestInfos.Count; i++)
        {
            // Info
            var request = data.RequestInfos[i];
            var info = FocusCharacterController.Instance.GetInfoFromPath(request);

            // Submission
            var map = maps[i];
            map.Submission = InventoryController.Instance.GetCharactersInInventory(new InventoryFilterOptions
            {
                ValidCharacters = new List<FocusCharacterInfo> { info },
                ExcludedDatas = Submissions
            }).FirstOrDefault();

            var has_submission = map.Submission != null;

            // Button
            var button = RequestButtons[i];
            button.SetObscured(!has_submission);
            button.Show();

            if (CurrentInfo.RequestPreviewHidden && !has_submission)
            {
                button.SetHiddenPreview();
            }
            else
            {
                button.SetCharacter(info);
            }

            if (i == 0)
            {
                ShowInfo(info);
            }
        }

        RewardPreviews.ForEach(x => x.Hide());
        if (data.MoneyReward > 0)
        {
            var preview = RewardPreviews[0];
            preview.SetObscured(CurrentInfo.HasMoneyReward);
            preview.Show();

            if (CurrentInfo.HasMoneyReward)
            {
                preview.SetCoinStack(data.MoneyReward);
            }
            else
            {
                preview.SetHiddenPreview();
            }
        }

        var already_unlocked = Item.IsOwned(CurrentInfo.ItemUnlock);
        var show_unlock_bar = HasItemUnlock && !already_unlocked;
        RewardUnlockBar.Visible = show_unlock_bar;
        if (show_unlock_bar)
        {
            RewardUnlockBar.Load(CurrentInfo);
        }

        Validate();
    }

    private void Process_TimerLabel()
    {
        if (CurrentData == null) return;

        var date_now = GameTime.GetCurrentDateTime();
        var date_next = GameTime.ParseDateTime(CurrentData.DateTimeNext);
        var span_next = date_next.Subtract(date_now);

        if (span_next >= TimeSpan.Zero)
        {
            TimerLabel.Text = span_next.ToString("hh':'mm':'ss");
        }
    }

    private void ShowInfo(FocusCharacterInfo info)
    {
        NameLabel.Text = Tr(info.Name);
        LocationHintLabel.Text = Tr(info.LocationHint);
    }

    private void RequestButton_FocusEntered(ButtonMap map)
    {
        var request = CurrentData.RequestInfos[map.Index];
        var info = FocusCharacterController.Instance.GetInfoFromPath(request);
        ShowInfo(info);
    }

    private void ClaimButton_Pressed()
    {
        Claim();
    }

    private void Claim()
    {
        CurrentClaimed = true;
        CurrentData.ClaimedCount++;

        foreach (var map in maps)
        {
            InventoryController.Instance.RemoveCharacterData(map.Submission);
        }

        if (CurrentInfo.HasMoneyReward && CurrentData.MoneyReward > 0)
        {
            Money.Add(CurrentData.MoneyReward);
        }

        if (HasItemUnlock && IsMaxClaim)
        {
            var data = Item.GetOrCreateData(CurrentInfo.ItemUnlock);
            data.Owned = true;
        }

        Data.Game.Save();
        HandInController.Instance.UnpinHandIn(CurrentInfo.Id);

        ClaimButton.Disabled = true;

        OnClaim?.Invoke();

    }

    private void Validate()
    {
        var is_valid = Submissions.Count == CurrentData.RequestInfos.Count;
        ClaimButton.Disabled = !is_valid;
    }

    public Button GetFirstButton()
    {
        return RequestButtons.First();
    }

    public IEnumerator WaitForRewardBarFill()
    {
        if (CurrentClaimed && RewardUnlockBar.Visible)
        {
            yield return RewardUnlockBar.WaitForFillNext();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void PinButton_Pressed()
    {
        HandInController.Instance.PinHandIn(CurrentInfo.Id);
    }
}
