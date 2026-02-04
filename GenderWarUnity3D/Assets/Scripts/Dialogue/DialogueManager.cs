using UnityEngine;
using System;
using System.Collections.Generic;
using GenderWar.Core;

namespace GenderWar.Dialogue
{
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

        [Header("Current State")]
        private DialogueDatabase currentDatabase;
        private DialogueNode currentNode;

        // Events
        public event Action<DialogueNode> OnNodeChanged;
        public event Action<DialogueChoice> OnChoiceMade;
        public event Action<DialogueNode> OnDialogueEnded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
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

            // Process dynamic text replacements
            ProcessDynamicText(currentNode);

            OnNodeChanged?.Invoke(currentNode);

            if (currentNode.IsEnding)
            {
                OnDialogueEnded?.Invoke(currentNode);
            }
        }

        public void MakeChoice(int choiceIndex)
        {
            if (currentNode == null || choiceIndex >= currentNode.Choices.Count) return;

            var choice = currentNode.Choices[choiceIndex];

            // Apply effects
            if (choice.Effects != null)
            {
                if (choice.Effects.MoneyChange != 0)
                {
                    GameManager.Instance.ModifyMoney(choice.Effects.MoneyChange);
                }
                if (choice.Effects.PatienceChange != 0)
                {
                    GameManager.Instance.ModifyPatience(choice.Effects.PatienceChange);
                }
            }

            // Add receipt lines
            if (choice.ReceiptLines != null)
            {
                foreach (var line in choice.ReceiptLines)
                {
                    GameManager.Instance.AddReceiptLine(line.Label, line.Cost);
                }
            }

            // Log choice for reputation
            GameManager.Instance.LogChoice(currentNode.Id, choice.Label, choice.Effects);

            OnChoiceMade?.Invoke(choice);

            // Navigate to next node
            if (!string.IsNullOrEmpty(choice.NextId))
            {
                // Handle special node IDs
                if (choice.NextId == "{INTEGRATION_ENDING}")
                {
                    HandleIntegrationEnding();
                }
                else
                {
                    LoadNode(choice.NextId);
                }
            }
        }

        private void ProcessDynamicText(DialogueNode node)
        {
            // Replace {RUMOR} placeholder
            if (node.Text.Contains("{RUMOR}"))
            {
                string rumor = GameManager.Instance.GetRumorLine();
                node.Text = node.Text.Replace("{RUMOR}", rumor);
            }

            // Replace {INTEGRATION_RESULT} placeholder
            if (node.Text.Contains("{INTEGRATION_RESULT}"))
            {
                var result = GameManager.Instance.CalculateIntegration();
                node.Text = node.Text.Replace("{INTEGRATION_RESULT}", result.ResultText);
            }
        }

        private void HandleIntegrationEnding()
        {
            var result = GameManager.Instance.CalculateIntegration();
            string endingNodeId = result.Accepted ? "ending_onboarded" : "ending_not_a_fit";
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
    public List<GenderWar.Core.ReceiptLine> EndingReceiptLines = new List<GenderWar.Core.ReceiptLine>();
}

[Serializable]
public class DialogueChoice
{
    public string Label;
    public string NextId;
    public GenderWar.Core.ChoiceEffects Effects;
    public List<GenderWar.Core.ReceiptLine> ReceiptLines = new List<GenderWar.Core.ReceiptLine>();
}
