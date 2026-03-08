using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class HandInContainer : ControlScript
{
    [Export]
    public Label RequestLabel;

    [Export]
    public InventoryContainer Inventory;

    [Export]
    public Button BackButton;

    [Export]
    public Button ClaimButton;

    [Export]
    public Button PinButton;

    [Export]
    public RewardUnlockBar RewardUnlockBar;

    private List<ButtonMap> maps = new();

    public event Action OnClaim;

    public HandInData CurrentData { get; private set; }
    public HandInInfo CurrentInfo { get; private set; }
    public HandInRequestInfo CurrentRequest { get; private set; }
    public bool IsClaimed { get; private set; }
    public bool HasItemUnlock => CurrentInfo?.HasItemUnlock ?? false;
    public bool IsMaxClaim => CurrentData?.ClaimCount >= CurrentInfo?.Requests.Count;

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
        Inventory.OnButtonPressed += InventoryButton_Pressed;
    }

    private void Clear()
    {
        maps.ForEach(x => x.Clear());
        ClaimButton.Disabled = true;
        CurrentData = null;
        RewardUnlockBar.Clear();
        IsClaimed = false;
        Inventory.Clear();
    }

    public void Load(HandInData data)
    {
        Clear();

        CurrentData = data;
        CurrentInfo = HandInController.Instance.GetInfo(data.Id);
        CurrentRequest = CurrentInfo.Requests.ToList().GetClamped(data.ClaimCount);
        IsClaimed = false;

        RequestLabel.Text = CurrentRequest.GetRequestText();

        Inventory.UpdateButtons();
        Inventory.SetMode(InventoryContainer.Mode.Select);

        RewardUnlockBar.Visible = HasItemUnlock;
    }

    private void ClaimButton_Pressed()
    {
        Claim();
    }

    private void Claim()
    {
        IsClaimed = true;
        CurrentData.ClaimCount++;

        foreach (var map in maps)
        {
            InventoryController.Instance.RemoveCharacterData(map.Submission);
        }

        if (CurrentRequest.HasMoney)
        {
            Money.Add(CurrentRequest.Money);
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
        var is_valid = Inventory.Selection.Count == CurrentRequest.Count;
        ClaimButton.Disabled = !is_valid;
    }

    public Button GetFirstButton()
    {
        return Inventory.GetFirstButton() ?? ClaimButton;
    }

    public IEnumerator WaitForRewardBarFill()
    {
        if (IsClaimed && RewardUnlockBar.Visible)
        {
            yield return RewardUnlockBar.WaitForFillNext();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void PinButton_Pressed()
    {
        HandInController.Instance.PinHandIn(CurrentInfo.Id);
    }

    private void InventoryButton_Pressed(InventoryCharacterData data)
    {
        Validate();
    }
}
