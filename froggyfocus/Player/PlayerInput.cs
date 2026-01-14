using Godot;

public partial class PlayerInput : Node
{
    public static CustomInputAction Forward = new CustomInputAction("Forward");
    public static CustomInputAction Back = new CustomInputAction("Back");
    public static CustomInputAction Left = new CustomInputAction("Left");
    public static CustomInputAction Right = new CustomInputAction("Right");
    public static CustomInputAction Jump = new CustomInputAction("Jump");
    public static CustomInputAction Interact = new CustomInputAction("Interact");
    public static CustomInputAction Focus = new CustomInputAction("Focus");
    public static CustomInputAction Pause = new CustomInputAction("Pause");
    public static CustomInputAction ZoomIn = new CustomInputAction("ZoomIn");
    public static CustomInputAction ZoomOut = new CustomInputAction("ZoomOut");
    public static CustomInputAction ZoomIn_Controller = new CustomInputAction("ZoomIn_Controller");
    public static CustomInputAction ZoomOut_Controller = new CustomInputAction("ZoomOut_Controller");

    public static Vector2 GetMoveInput()
    {
        return Input.GetVector(
            Left.Name,
            Right.Name,
            Forward.Name,
            Back.Name);
    }

    public static Vector2 GetLookInput()
    {
        return Input.GetVector("LookLeft", "LookRight", "LookUp", "LookDown");
    }
}
