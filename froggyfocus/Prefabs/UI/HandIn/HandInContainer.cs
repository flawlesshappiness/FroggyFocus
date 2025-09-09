using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class HandInContainer : MarginContainer
{
    [Export]
    public Button BackButton;

    [Export]
    public Button ClaimButton;

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
    public bool HasItemUnlock => (CurrentInfo?.HatUnlock ?? AppearanceHatType.None) != AppearanceHatType.None;
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

        InitializeRequestButtons();
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
    }

    public void Load(HandInData data)
    {
        Clear();

        CurrentData = data;
        CurrentInfo = HandInController.Instance.GetInfo(data.Id);
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
            preview.SetCoinStack(data.MoneyReward);
            preview.Show();
        }

        var already_unlocked = Data.Game.Appearance.PurchasedHats.Contains(CurrentInfo.HatUnlock);
        var show_unlock_bar = HasItemUnlock && !already_unlocked;
        RewardUnlockBar.Visible = show_unlock_bar;
        if (show_unlock_bar)
        {
            RewardUnlockBar.Load(CurrentInfo);
        }

        Validate();
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
        CurrentData.Claimed = true;
        CurrentData.ClaimedCount++;

        foreach (var map in maps)
        {
            InventoryController.Instance.RemoveCharacterData(map.Submission);
        }

        if (CurrentData.MoneyReward > 0)
        {
            Money.Add(CurrentData.MoneyReward);
        }

        if (HasItemUnlock && IsMaxClaim)
        {
            AppearanceHatController.Instance.Unlock(CurrentInfo.HatUnlock);
            AppearanceHatController.Instance.Purchase(CurrentInfo.HatUnlock);
        }

        Data.Game.Save();

        ClaimButton.Disabled = true;

        //Close();
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
        if (CurrentData.Claimed && RewardUnlockBar.Visible)
        {
            yield return RewardUnlockBar.WaitForFillNext();
            yield return new WaitForSeconds(0.25f);
        }
    }
}
