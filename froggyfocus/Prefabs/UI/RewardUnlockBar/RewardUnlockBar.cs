using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class RewardUnlockBar : Control
{
    [Export]
    public ProgressBar BarPieceTemplate;

    [Export]
    public RewardPreview RewardPreview;

    [Export]
    public Label Label;

    private int current_value;
    private List<ProgressBar> bar_pieces = new();

    public void Clear()
    {
        BarPieceTemplate.Hide();
        bar_pieces.ForEach(x => x.QueueFree());
        bar_pieces.Clear();
    }

    public void Load(HandInInfo info)
    {
        Clear();

        var data = HandIn.GetOrCreateData(info.Id);
        SetBar(data.ClaimedCount, info.ClaimCountToUnlock);

        var hat_info = AppearanceHatController.Instance.GetInfo(info.HatUnlock);
        RewardPreview.SetHat(hat_info);
    }

    public void SetBar(int value, int max)
    {
        Clear();

        current_value = value;
        var empty_count = max - value;

        Label.Text = $"Claim {empty_count} more times to unlock";

        for (int i = 0; i < max; i++)
        {
            CreateBarPiece();
        }

        for (int i = 0; i < Mathf.Min(value, max); i++)
        {
            var piece = bar_pieces[i];
            piece.Value = piece.MaxValue;
        }
    }

    private void CreateBarPiece()
    {
        BarPieceTemplate.Hide();
        var parent = BarPieceTemplate.GetParent();
        var bar = BarPieceTemplate.Duplicate() as ProgressBar;
        bar.SetParent(parent);
        bar.Show();
        bar_pieces.Add(bar);
    }

    public IEnumerator WaitForFillNext()
    {
        var piece = bar_pieces.GetClamped(current_value);
        var curve = Curves.EaseOutQuad;
        yield return LerpEnumerator.Lerp01(0.5f, f =>
        {
            piece.Value = curve.Evaluate(f);
        });
    }
}
