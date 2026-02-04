using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages dialogue flow, node progression, and choice handling
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue Data")]
    public DialogueDatabase IncelRoute;
    public DialogueDatabase FemcelRoute;
    public DialogueDatabase PerformativeRoute;
    public DialogueDatabase BopRoute;
    public DialogueDatabase ThemcelRoute;

    private DialogueDatabase currentDatabase;
    private DialogueNode currentNode;

    public event Action<DialogueNode> OnNodeChanged;
    public event Action<DialogueChoice> OnChoiceMade;
    public event Action<DialogueNode> OnDialogueEnded;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartDialogue(RouteType route)
    {
        currentDatabase = GetDatabase(route);
        if (currentDatabase == null)
        {
            Debug.LogError($"No dialogue database found for route: {route}");
            return;
        }
        LoadNode("start");
    }

    public void LoadNode(string nodeId)
    {
        if (currentDatabase == null) return;

        currentNode = currentDatabase.GetNode(nodeId);
        if (currentNode == null)
        {
            Debug.LogError($"Node not found: {nodeId}");
            return;
        }

        ProcessDynamicText(currentNode);
        OnNodeChanged?.Invoke(currentNode);

        if (currentNode.IsEnding)
            OnDialogueEnded?.Invoke(currentNode);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (currentNode == null || choiceIndex >= currentNode.Choices.Count) return;

        var choice = currentNode.Choices[choiceIndex];

        if (choice.Effects != null)
        {
            if (choice.Effects.MoneyChange != 0)
                GameManager.Instance?.ModifyMoney(choice.Effects.MoneyChange);
            if (choice.Effects.PatienceChange != 0)
                GameManager.Instance?.ModifyPatience(choice.Effects.PatienceChange);
        }

        if (choice.ReceiptLines != null)
        {
            foreach (var line in choice.ReceiptLines)
                GameManager.Instance?.AddReceiptLine(line.Label, line.Cost);
        }

        GameManager.Instance?.LogChoice(currentNode.Id, choice.Label, choice.Effects);
        OnChoiceMade?.Invoke(choice);

        if (!string.IsNullOrEmpty(choice.NextId))
        {
            if (choice.NextId == "{INTEGRATION_ENDING}")
                HandleIntegrationEnding();
            else
                LoadNode(choice.NextId);
        }
    }

    private void ProcessDynamicText(DialogueNode node)
    {
        if (node.Text.Contains("{RUMOR}"))
        {
            string rumor = GameManager.Instance?.GetRumorLine() ?? "";
            node.Text = node.Text.Replace("{RUMOR}", rumor);
        }

        if (node.Text.Contains("{INTEGRATION_RESULT}"))
        {
            var result = GameManager.Instance?.CalculateIntegration();
            node.Text = node.Text.Replace("{INTEGRATION_RESULT}", result?.ResultText ?? "");
        }
    }

    private void HandleIntegrationEnding()
    {
        var result = GameManager.Instance?.CalculateIntegration();
        string endingNodeId = (result?.Accepted ?? false) ? "ending_onboarded" : "ending_not_a_fit";
        LoadNode(endingNodeId);
    }

    private DialogueDatabase GetDatabase(RouteType route)
    {
        return route switch
        {
            RouteType.Incel => IncelRoute,
            RouteType.Femcel => FemcelRoute,
            RouteType.Performative => PerformativeRoute,
            RouteType.Bop => BopRoute,
            RouteType.Themcel => ThemcelRoute,
            _ => null
        };
    }

    public DialogueNode GetCurrentNode() => currentNode;
}

[Serializable]
public class DialogueNode
{
    public string Id;
    public string Speaker;
    public string Text;
    public string DateExpression = "neutral";
    public string PlayerExpression = "neutral";
    public List<DialogueChoice> Choices = new List<DialogueChoice>();
    public bool IsEnding = false;
    public string EndingTitle;
    public string EndingText;
    public List<ReceiptLine> EndingReceiptLines = new List<ReceiptLine>();
}

[Serializable]
public class DialogueChoice
{
    public string Label;
    public string NextId;
    public ChoiceEffects Effects;
    public List<ReceiptLine> ReceiptLines = new List<ReceiptLine>();
}
