using Godot;
using System;

[GlobalClass]
public partial class InteractArea : Area3D, IInteractable
{
    public event Action OnInteract;

    public void Interact()
    {
        OnInteract?.Invoke();
    }
}
