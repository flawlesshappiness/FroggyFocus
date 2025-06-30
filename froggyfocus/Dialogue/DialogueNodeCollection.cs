using Godot;
using System.Collections.Generic;
using System.Text.Json;

public class DialogueNodeCollection
{
    public Dictionary<string, DialogueNode> Nodes = new();

    public DialogueNodeCollection(string path)
    {
        Deserialize(path);
    }

    private void Deserialize(string path)
    {
        Nodes.Clear();
        var content = FileAccess.GetFileAsString(path);
        var nodes = JsonSerializer.Deserialize<IEnumerable<DialogueNode>>(content);
        nodes.ForEach(x => Nodes.Add(x.id, x));
    }

    public DialogueNode GetNode(string name)
    {
        return Nodes.TryGetValue(name, out var node) ? node : null;
    }
}
