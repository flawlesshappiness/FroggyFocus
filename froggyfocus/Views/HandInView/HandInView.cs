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
    public AnimatedPanel AnimatedPanel_Inventory;

    [Export]
    public Button CloseHandInButton;

    [Export]
    public Button CloseInventoryButton;

    [Export]
    public Button ClaimButton;

    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Control InputBlocker;

    [Export]
    public AudioStreamPlayer SfxMoney;

    [Export]
    public Array<InventoryPreviewButton> RequestButtons;

    [Export]
    public Array<RewardPreview> RewardPreviews;

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

        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
        CloseHandInButton.Pressed += CloseHandInButton_Pressed;
        CloseInventoryButton.Pressed += CloseInventoryButton_Pressed;
        ClaimButton.Pressed += ClaimButton_Pressed;

        InitializeRequestButtons();

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = nameof(HandInView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = DebugShow
        });

        void DebugShow(DebugView v)
        {
            var request_info = FocusCharacterController.Instance.Collection.Resources.First();

            ShowPopup(new HandInData
            {
                Requests = new List<InventoryCharacterData>
                {
                    new InventoryCharacterData
                    {
                        InfoPath = request_info.ResourcePath,
                    }
                },
                MoneyReward = 50,
            });

            v.Close();
        }
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

            button.Pressed += () => RequestButton_Pressed(map);
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

    public void ShowPopup(string id)
    {
        var data = Data.Game.HandIns.FirstOrDefault(x => x.Id == id);
        ShowPopup(data);
    }

    public void ShowPopup(HandInData data)
    {
        if (data == null) return;

        Clear();
        Load(data);
        Show();

        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindShow();
            yield return AnimatedPanel_HandIn.AnimatePopShow();
            RequestButtons.First().GrabFocus();
            InputBlocker.Hide();
        }
    }

    private void Clear()
    {
        maps.ForEach(x => x.Clear());
        ClaimButton.Disabled = true;
    }

    private void Load(HandInData data)
    {
        current_data = data;
        RequestButtons.ForEach(x => x.Hide());

        for (int i = 0; i < data.Requests.Count; i++)
        {
            var request = data.Requests[i];
            var info = FocusCharacterController.Instance.GetInfoFromPath(request.InfoPath);
            var button = RequestButtons[i];
            button.SetCharacter(info);
            button.SetObscured(true);
            button.Show();
        }

        RewardPreviews.ForEach(x => x.Hide());
        if (data.MoneyReward > 0)
        {
            var preview = RewardPreviews[0];
            preview.SetCoinStack();
            preview.SetAmount(data.MoneyReward);
            preview.Show();
        }
    }

    private void SetLocks(bool locked)
    {
        Player.SetAllLocks(nameof(HandInView), locked);
        MouseVisibility.Instance.Lock.SetLock(nameof(HandInView), locked);
    }

    private void RequestButton_Pressed(ButtonMap map)
    {
        var request = current_data.Requests[map.Index];
        var info = FocusCharacterController.Instance.GetInfoFromPath(request.InfoPath);

        selected_map = map;
        InventoryContainer.UpdateButtons(new InventoryContainer.FilterOptions
        {
            ExcludedDatas = GetSubmissions(),
            ValidCharacters = new List<FocusCharacterInfo> { info },
        });

        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_HandIn.AnimateShrink();
            yield return AnimatedPanel_Inventory.AnimatePopShow();
            InputBlocker.Hide();

            var button = InventoryContainer.GetFirstButton() ?? CloseInventoryButton;
            button?.GrabFocus();
        }
    }

    private void InventoryButton_Pressed(FocusCharacterInfo info)
    {
        var data = InventoryContainer.GetSelectedData();
        selected_map.Submission = data;

        selected_map.Button.SetCharacter(info);
        selected_map.Button.SetObscured(false);
        ClaimButton.Disabled = !Validate();
        CloseInventoryButton_Pressed();
    }

    private void CloseHandInButton_Pressed()
    {
        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel_HandIn.AnimatePopHide();
            InputBlocker.Hide();
            Hide();
        }
    }

    private void CloseInventoryButton_Pressed()
    {
        StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();

            InputBlocker.Show();
            AnimatedPanel_HandIn.AnimateGrow();
            yield return AnimatedPanel_Inventory.AnimatePopHide();
            InputBlocker.Hide();
            selected_map.Button?.GrabFocus();
        }
    }

    private void ClaimButton_Pressed()
    {
        TryClaim();
    }

    private void TryClaim()
    {
        if (Validate())
        {
            Claim();
        }
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
            SfxMoney.Play();
        }

        current_data.Claimed = true;
        CloseHandInButton_Pressed();
    }

    private bool Validate()
    {
        return GetSubmissions().Count == current_data.Requests.Count;
    }

    private List<InventoryCharacterData> GetSubmissions()
    {
        return maps
            .Select(x => x.Submission)
            .Where(x => x != null)
            .ToList();
    }
}
