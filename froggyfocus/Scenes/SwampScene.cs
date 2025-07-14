public partial class SwampScene : GameScene
{
    public override void _Ready()
    {
        base._Ready();
        MusicController.Instance.StartMusic();
    }
}
