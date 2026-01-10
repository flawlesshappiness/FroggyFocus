using Godot;

public partial class SaveProfileControl : MarginContainer
{
    [Export]
    public Label TitleLabel;

    [Export]
    public Label NoDataLabel;

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
    private GameSaveData data;

    public override void _Ready()
    {
        base._Ready();
        ProfileButton.Pressed += Profile_Pressed;
        DeleteButton.Pressed += Delete_Pressed;
        ConfirmDeleteButton.Pressed += ConfirmDelete_Pressed;
        CancelDeleteButton.Pressed += CancelDelete_Pressed;
        VisibilityChanged += OnVisibilityChanged;
        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
    }

    public void LoadData(int profile)
    {
        this.profile = profile;
        TitleLabel.Text = $"{profile}";

        data = GameProfileController.Instance.GetGameProfile(profile);
        LoadData(data);

        Frog.Visible = !data.Deleted;
        NoDataLabel.Visible = data.Deleted;
    }

    private void LoadData(GameSaveData data)
    {
        if (data == null) return;
        Frog.LoadAppearance(data);
    }

    private void ProfileSelected(int selected_profile)
    {
        TitleLabel.Modulate = selected_profile == profile ? Colors.White : Colors.Black.SetA(0.5f);
    }

    private void Profile_Pressed()
    {
        GameProfileController.Instance.SelectGameProfile(profile);
        Data.Options.Save();
    }

    private void ClearDeleteButtons()
    {
        DeleteButton.Visible = !data.Deleted;
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
