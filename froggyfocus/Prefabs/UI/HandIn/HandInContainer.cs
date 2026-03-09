using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class HandInContainer : ControlScript
{
    [Export]
    public Label RequestLabel;

    [Export]
    public RewardPreview MoneyPreview;

    [Export]
    public InventoryContainer Inventory;

    [Export]
    public InventoryInfoContainer InventoryInfo;

    [Export]
    public Button BackButton;

    [Export]
    public Button ClaimButton;

    [Export]
    public Button PinButton;

    [Export]
    public ProgressBar ProgressBar;

    [Export]
    public Label ProgressLabel;

    [Export]
    public RewardUnlockBar RewardUnlockBar;

    public event Action OnClaim;

    public HandInData CurrentData { get; private set; }
    public HandInInfo CurrentInfo { get; private set; }
    public HandInRequestInfo CurrentRequest { get; private set; }
    public bool IsClaimed { get; private set; }
    public bool IsClaimedItem { get; private set; }
    public bool HasItemUnlock => CurrentInfo?.HasItemUnlock ?? false;
    public bool IsMaxClaim => CurrentData?.ClaimCount >= CurrentInfo?.Requests.Count;

    public override void _Ready()
    {
        base._Ready();

        ClaimButton.Pressed += ClaimButton_Pressed;
        PinButton.Pressed += PinButton_Pressed;
        Inventory.OnButtonPressed += InventoryButton_Pressed;
        Inventory.OnButtonFocus += InventoryButton_Focus;
    }

    private void Clear()
    {
        CurrentData = null;
        RewardUnlockBar.Clear();
        IsClaimed = false;
        IsClaimedItem = false;
        Inventory.Clear();
        InventoryInfo.Clear();
        MoneyPreview.Hide();

        ProgressBar.Value = 0;
        ProgressBar.Show();
        ClaimButton.Hide();
        ClaimButton.Disabled = true;
    }

    public void Load(HandInData data)
    {
        Clear();

        CurrentData = data;
        CurrentInfo = HandInController.Instance.GetInfo(data.Id);
        CurrentRequest = CurrentInfo.Requests.ToList().GetClamped(data.ClaimCount);
        IsClaimed = false;

        RequestLabel.Text = CurrentRequest.GetRequestText();

        Inventory.UpdateButtons(CurrentRequest.GetInventoryFilterOptions());
        Inventory.SetMode(InventoryContainer.Mode.Select);

        RewardUnlockBar.Load(CurrentInfo);
        RewardUnlockBar.Visible = HasItemUnlock;

        if (CurrentRequest.HasMoney)
        {
            MoneyPreview.Show();
            MoneyPreview.SetCoinStack(CurrentRequest.Money);
        }

        Validate();
    }

    private void ClaimButton_Pressed()
    {
        Claim();
    }

    private void Claim()
    {
        IsClaimed = true;
        CurrentData.ClaimCount++;

        foreach (var data in Inventory.Selection)
        {
            InventoryController.Instance.RemoveCharacterData(data);
        }

        if (CurrentRequest.HasMoney)
        {
            Money.Add(CurrentRequest.Money);
        }

        if (HasItemUnlock && IsMaxClaim)
        {
            var data = Item.GetOrCreateData(CurrentInfo.ItemUnlock);
            data.Owned = true;
            IsClaimedItem = true;
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
        ClaimButton.Visible = is_valid;

        ProgressBar.Visible = !is_valid;
        ProgressLabel.Text = $"{Inventory.Selection.Count} / {CurrentRequest.Count}";

        AnimateProgressBar();
    }

    private void AnimateProgressBar()
    {
        this.StartCoroutine(Cr, "progress");
        IEnumerator Cr()
        {
            var start = ProgressBar.Value;
            var end = Inventory.Selection.Count / (float)CurrentRequest.Count;
            var curve = Curves.EaseOutQuad;
            yield return LerpEnumerator.Lerp01(0.2f, f =>
            {
                var t = curve.Evaluate(f);
                ProgressBar.Value = Mathf.Lerp(start, end, t);
            });
        }
    }

    public Button GetFocusButton()
    {
        return BackButton;
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

    private void InventoryButton_Focus(InventoryCharacterData data)
    {
        InventoryInfo.SetCharacter(data);
    }
}
