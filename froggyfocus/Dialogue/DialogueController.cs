using System;

public partial class DialogueController : SingletonController
{
    public override string Directory => "Dialogue";
    public static DialogueController Instance => Singleton.Get<DialogueController>();
    private DialogueView View => Singleton.Get<DialogueView>();
    private DialogueNodeCollection Collection { get; set; }

    private readonly string json_path = "res://Dialogue/dialogue.json";

    public event Action<string> OnNodeStarted;
    public event Action<string> OnNodeEnded;
    public event Action OnDialogueStarted;
    public event Action OnDialogueEnded;

    private DialogueNode current_node;

    protected override void Initialize()
    {
        base.Initialize();
        Collection = new DialogueNodeCollection(json_path);

        View.OnNextDialogue += NextDialogue;

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "DIALOGUE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Test dialogue",
            Action = DebugTest
        });

        void DebugTest(DebugView v)
        {
            v.Close();
            StartDialogue("##TEST_001##");
        }
    }

    public void StartDialogue(string id)
    {
        SetNode(id);
    }

    private void SetNode(string id)
    {
        var node = Collection.GetNode(id);
        SetNode(node);
    }

    private void SetNode(DialogueNode node)
    {
        if (current_node == null)
        {
            OnDialogueStarted?.Invoke();
        }
        else
        {
            OnNodeEnded?.Invoke(current_node.id);
        }

        current_node = node;
        View.AnimateDialogue(Tr(node.id));

        OnNodeStarted?.Invoke(node.id);
    }

    private void NextDialogue()
    {
        if (string.IsNullOrEmpty(current_node?.next))
        {
            current_node = null;
            View.AnimateClose();

            OnDialogueEnded?.Invoke();
        }
        else
        {
            SetNode(current_node.next);
        }
    }
}
