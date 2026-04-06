using System;

public partial class DialogueController : SingletonController
{
    public override string Directory => "Dialogue";
    public static DialogueController Instance => Singleton.Get<DialogueController>();
    private DialogueView View => Singleton.Get<DialogueView>();
    public DialogueNodeCollection Collection { get; private set; }

    private readonly string json_path = "res://Dialogue/dialogue.json";

    public event Action<string> OnEntryStarted;
    public event Action<string> OnEntryEnded;
    public event Action<string> OnDialogueStarted;
    public event Action<string> OnDialogueEnded;

    private DialogueNode current_node;
    private int current_entry_index;

    protected override void Initialize()
    {
        base.Initialize();
        Collection = new DialogueNodeCollection(json_path);

        View.OnNextDialogue += NextEntry;

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "DIALOGUE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Start dialogue",
            Action = ListDialogueIds
        });

        void ListDialogueIds(DebugView v)
        {
            v.SetContent_Search();

            foreach (var node in Collection.Nodes)
            {
                v.ContentSearch.AddItem($"{node.Key} ({node.Value.entries?.Length})", () => DebugStartDialogue(v, node.Key));
            }

            v.ContentSearch.UpdateButtons();
        }

        void DebugStartDialogue(DebugView v, string id)
        {
            v.Close();
            StartDialogue(id);
        }
    }

    public void StartDialogue(string id)
    {
        Debug.TraceMethod(id);

        id = id.Replace("#", "");
        id = $"##{id}##";

        current_entry_index = 0;
        SetNode(id);
    }

    private void SetNode(string id)
    {
        Debug.TraceMethod(id);
        var node = Collection.GetNode(id) ?? new DialogueNode
        {
            id = id,
            entries = [id]
        };

        SetNode(node);
    }

    private void SetNode(DialogueNode node)
    {
        Debug.TraceMethod(node);
        if (node == null)
        {
            Debug.LogError("DialogueController.SetNode(): Node was null");
            return;
        }

        if (current_node == null)
        {
            OnDialogueStarted?.Invoke(node.id.Replace("#", ""));
        }

        current_node = node;
        StartEntry(node, current_entry_index);
    }

    private void StartEntry(DialogueNode node, int entry_index)
    {
        var entry = node.entries[entry_index];
        var text = Tr(entry);
        View.AnimateDialogue(text);

        OnEntryStarted?.Invoke(entry.Replace("#", ""));
    }

    private void NextEntry()
    {
        if (current_node != null)
        {
            OnEntryEnded?.Invoke(current_node.id.Replace("#", ""));
        }

        current_entry_index++;
        if (current_entry_index < current_node?.entries?.Length)
        {
            StartEntry(current_node, current_entry_index);
        }
        else
        {
            var id = current_node?.id ?? string.Empty;
            current_node = null;
            current_entry_index = 0;
            View.AnimateClose();

            OnDialogueEnded?.Invoke(id.Replace("#", ""));
        }
    }
}
