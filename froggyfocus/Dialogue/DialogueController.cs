using System;
using System.Collections.Generic;

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
    private Dictionary<string, string> dialogue_variables = new();

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
        Debug.TraceMethod(id);
        SetNode(id);
    }

    private void SetNode(string id)
    {
        Debug.TraceMethod(id);
        var node = Collection.GetNode(id);
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
            OnDialogueStarted?.Invoke();
        }

        current_node = node;

        var text = FormatText(Tr(node.id));
        View.AnimateDialogue(text);

        OnNodeStarted?.Invoke(node.id);
    }

    private void NextDialogue()
    {
        if (current_node != null)
        {
            OnNodeEnded?.Invoke(current_node.id);
        }

        if (string.IsNullOrEmpty(current_node?.next))
        {
            current_node = null;
            ClearVariables();
            View.AnimateClose();
            OnDialogueEnded?.Invoke();
        }
        else
        {
            SetNode(current_node.next);
        }
    }

    private void ClearVariables()
    {
        dialogue_variables.Clear();
    }

    public void AddVariable(string key, string value)
    {
        dialogue_variables.Add(key, value);
    }

    private string FormatText(string text)
    {
        foreach (var variable in dialogue_variables)
        {
            if (text.Contains(variable.Key))
            {
                text = text.Replace(variable.Key, Tr(variable.Value));
            }
        }

        return text;
    }
}
