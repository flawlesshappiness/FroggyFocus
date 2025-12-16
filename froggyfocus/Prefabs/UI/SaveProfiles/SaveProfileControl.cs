using Godot;

public partial class SaveProfileControl : MarginContainer
{
    [Export]
    public Label TitleLabel;

    [Export]
    public Button ProfileButton;

    [Export]
    public Button DeleteButton;

    [Export]
    public Button ConfirmDeleteButton;

    [Export]
    public Button CancelDeleteButton;

    [Export]
    public FrogCharacter Frog;

    private int profile;

    public override void _Ready()
    {
        base._Ready();
        ProfileButton.Pressed += Profile_Pressed;
        DeleteButton.Pressed += Delete_Pressed;
        ConfirmDeleteButton.Pressed += ConfirmDelete_Pressed;
        CancelDeleteButton.Pressed += CancelDelete_Pressed;
        VisibilityChanged += OnVisibilityChanged;
    }

    public void LoadData(int profile)
    {
        this.profile = profile;
        TitleLabel.Text = $"{profile}";

        var data = GameProfileController.Instance.GetGameProfile(profile);
        LoadData(data);
    }

    private void LoadData(GameSaveData data)
    {
        if (data == null) return;
        Frog.LoadAppearance(data);
    }

    private void Profile_Pressed()
    {
        GameProfileController.Instance.SelectGameProfile(profile);
        Data.Options.Save();
    }

    private void ClearDeleteButtons()
    {
        DeleteButton.Show();
        ConfirmDeleteButton.Hide();
        CancelDeleteButton.Hide();
    }

    private void Delete_Pressed()
    {
        DeleteButton.Hide();
        ConfirmDeleteButton.Show();
        CancelDeleteButton.Show();

        CancelDeleteButton.GrabFocus();
    }

    private void CancelDelete_Pressed()
    {
        ClearDeleteButtons();
        ProfileButton.GrabFocus();
    }

    private void ConfirmDelete_Pressed()
    {
        GameProfileController.Instance.DeleteGameProfile(profile);
        LoadData(profile);

        // Reselect if this was selected
        if (Data.Options.SelectedGameProfile == profile)
        {
            GameProfileController.Instance.SelectGameProfile(profile);
        }

        ClearDeleteButtons();
        ProfileButton.GrabFocus();
    }

    private void OnVisibilityChanged()
    {
        if (!IsVisibleInTree()) return;
        ClearDeleteButtons();
    }
}
