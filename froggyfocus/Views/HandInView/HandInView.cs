using Godot;
using Godot.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class HandInView : View
{
    public static HandInView Instance => Get<HandInView>();

    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public AnimatedPanel AnimatedPanel_HandIn;

    [Export]
    public Button CloseHandInButton;

    [Export]
    public Button ClaimButton;

    [Export]
    public Label NameLabel;

    [Export]
    public RichTextLabel LocationHintLabel;

    [Export]
    public ShopExpandPopup ShopExpandPopup;

    [Export]
    public RewardUnlockBar RewardUnlockBar;

    [Export]
    public Control InputBlocker;

    [Export]
    public Array<InventoryPreviewButton> RequestButtons;

    [Export]
    public Array<RewardPreview> RewardPreviews;

    private bool animating;
    private HandInData current_data;
    private ButtonMap selected_map;
    private List<ButtonMap> maps = new();

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

        CloseHandInButton.Pressed += CloseHandInButton_Pressed;
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

            //button.Pressed += () => RequestButton_Pressed(map);
            button.FocusEntered += () => RequestButton_FocusEntered(map);
            maps.Add(map);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        SetLocks(true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        SetLocks(false);
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree() && !animating)
        {
            Close();
        }
    }

    public void ShowPopup(string id)
    {
        var data = HandIn.GetOrCreateData(id);
        ShowPopup(data);
    }

    public void ShowPopup(HandInData data)
    {
        if (data == null) return;

        Clear();
        Load(data);
        Validate();
        Show();

        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;

            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindShow();
            yield return AnimatedPanel_HandIn.AnimatePopShow();
            RequestButtons.First().GrabFocus();
            InputBlocker.Hide();

            animating = false;
        }
    }

    private void Clear()
    {
        maps.ForEach(x => x.Clear());
        ClaimButton.Disabled = true;
        current_data = null;
        RewardUnlockBar.Clear();
    }

    private void Load(HandInData data)
    {
        current_data = data;
        RequestButtons.ForEach(x => x.Hide());

        for (int i = 0; i < data.Requests.Count; i++)
        {
            // Info
            var request = data.Requests[i];
            var info = FocusCharacterController.Instance.GetInfoFromPath(request.InfoPath);

            // Submission
            var map = maps[i];
            map.Submission = InventoryController.Instance.GetCharactersInInventory(new InventoryFilterOptions
            {
                ValidCharacters = new List<FocusCharacterInfo> { info },
                ExcludedDatas = GetSubmissions()
            }).FirstOrDefault();

            // Button
            var button = RequestButtons[i];
            button.SetCharacter(info);
            button.SetObscured(map.Submission == null);
            button.Show();

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

        var handin_info = HandInController.Instance.GetInfo(data.Id);
        var has_unlock = handin_info.HatUnlock != AppearanceHatType.None;
        RewardUnlockBar.Visible = has_unlock;
        if (has_unlock)
        {
            RewardUnlockBar.Load(handin_info);
        }
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(HandInView);
        Player.SetAllLocks(id, locked);
        PauseView.ToggleLock.SetLock(id, locked);
        MouseVisibility.Instance.Lock.SetLock(id, locked);
    }

    private void RequestButton_FocusEntered(ButtonMap map)
    {
        var request = current_data.Requests[map.Index];
        var info = FocusCharacterController.Instance.GetInfoFromPath(request.InfoPath);
        ShowInfo(info);
    }

    private void ShowInfo(FocusCharacterInfo info)
    {
        NameLabel.Text = Tr(info.Name);
        LocationHintLabel.Text = Tr(info.LocationHint);
    }

    private void CloseHandInButton_Pressed()
    {
        Close();
    }

    private Coroutine Close()
    {
        return StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;

            ReleaseCurrentFocus();
            InputBlocker.Show();
            yield return WaitForRewardBarFill();
            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel_HandIn.AnimatePopHide();
            InputBlocker.Hide();
            yield return WaitForPopup();
            Hide();

            animating = false;
        }
    }

    private IEnumerator WaitForPopup()
    {
        if (current_data.Claimed)
        {
            if (current_data.HatUnlock != AppearanceHatType.None)
            {
                ShopExpandPopup.SetHat(current_data.HatUnlock);
                yield return ShopExpandPopup.WaitForPopup();
            }

            HandInController.Instance.HandInClaimed(current_data);
        }
    }

    private IEnumerator WaitForRewardBarFill()
    {
        if (current_data.Claimed)
        {
            yield return RewardUnlockBar.WaitForFillNext();
            yield return new WaitForSeconds(0.25f);
        }
    }

    private void ClaimButton_Pressed()
    {
        Claim();
    }

    private void Claim()
    {
        foreach (var map in maps)
        {
            InventoryController.Instance.RemoveCharacterData(map.Submission);
        }

        if (current_data.MoneyReward > 0)
        {
            Money.Add(current_data.MoneyReward);
        }

        if (current_data.HatUnlock != AppearanceHatType.None)
        {
            AppearanceHatController.Instance.Unlock(current_data.HatUnlock);
        }

        current_data.Claimed = true;
        current_data.ClaimedCount++;

        Data.Game.Save();

        ClaimButton.Disabled = true;

        Close();
    }

    private void Validate()
    {
        var is_valid = GetSubmissions().Count == current_data.Requests.Count;
        ClaimButton.Disabled = !is_valid;
    }

    private List<InventoryCharacterData> GetSubmissions()
    {
        return maps
            .Select(x => x.Submission)
            .Where(x => x != null)
            .ToList();
    }
}
