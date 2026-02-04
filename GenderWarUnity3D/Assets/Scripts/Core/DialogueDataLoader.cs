using UnityEngine;
using System.Collections.Generic;
using GenderWar.Dialogue;

namespace GenderWar.Core
{
    /// <summary>
    /// Loads dialogue data from JSON files into DialogueDatabase ScriptableObjects
    /// </summary>
    public class DialogueDataLoader : MonoBehaviour
    {
        [Header("JSON Files")]
        public TextAsset IncelRouteJson;
        public TextAsset FemcelRouteJson;
        public TextAsset PerformativeRouteJson;
        public TextAsset BopRouteJson;
        public TextAsset ThemcelRouteJson;

        [Header("Dialogue Databases")]
        public DialogueDatabase IncelDatabase;
        public DialogueDatabase FemcelDatabase;
        public DialogueDatabase PerformativeDatabase;
        public DialogueDatabase BopDatabase;
        public DialogueDatabase ThemcelDatabase;

        public void LoadAllDialogues()
        {
            if (IncelRouteJson != null && IncelDatabase != null)
            {
                LoadDialogueFromJson(IncelRouteJson.text, IncelDatabase);
            }

            if (FemcelRouteJson != null && FemcelDatabase != null)
            {
                LoadDialogueFromJson(FemcelRouteJson.text, FemcelDatabase);
            }

            if (PerformativeRouteJson != null && PerformativeDatabase != null)
            {
                LoadDialogueFromJson(PerformativeRouteJson.text, PerformativeDatabase);
            }

            if (BopRouteJson != null && BopDatabase != null)
            {
                LoadDialogueFromJson(BopRouteJson.text, BopDatabase);
            }

            if (ThemcelRouteJson != null && ThemcelDatabase != null)
            {
                LoadDialogueFromJson(ThemcelRouteJson.text, ThemcelDatabase);
            }
        }

        private void LoadDialogueFromJson(string json, DialogueDatabase database)
        {
            var data = JsonUtility.FromJson<DialogueRouteData>(json);
            if (data == null) return;

            database.RouteName = data.routeName;
            database.RouteType = ParseRouteType(data.routeType);
            database.Nodes.Clear();

            foreach (var nodeData in data.nodes)
            {
                var node = new DialogueNode
                {
                    Id = nodeData.id,
                    Speaker = nodeData.speaker,
                    Text = nodeData.text,
                    DateExpression = nodeData.dateExpression ?? "neutral",
                    PlayerExpression = nodeData.playerExpression ?? "neutral",
                    IsEnding = nodeData.isEnding,
                    EndingTitle = nodeData.endingTitle,
                    EndingText = nodeData.endingText
                };

                // Parse choices
                if (nodeData.choices != null)
                {
                    foreach (var choiceData in nodeData.choices)
                    {
                        var choice = new DialogueChoice
                        {
                            Label = choiceData.label,
                            NextId = choiceData.nextId
                        };

                        // Parse effects
                        if (choiceData.effects != null)
                        {
                            choice.Effects = new ChoiceEffects
                            {
                                MoneyChange = choiceData.effects.moneyChange,
                                PatienceChange = choiceData.effects.patienceChange,
                                Tags = choiceData.effects.tags != null
                                    ? new List<string>(choiceData.effects.tags)
                                    : new List<string>()
                            };
                        }

                        // Parse receipt lines
                        if (choiceData.receiptLines != null)
                        {
                            choice.ReceiptLines = new List<ReceiptLine>();
                            foreach (var line in choiceData.receiptLines)
                            {
                                choice.ReceiptLines.Add(new ReceiptLine(line.label, line.cost));
                            }
                        }

                        node.Choices.Add(choice);
                    }
                }

                // Parse ending receipt lines
                if (nodeData.endingReceiptLines != null)
                {
                    foreach (var line in nodeData.endingReceiptLines)
                    {
                        node.EndingReceiptLines.Add(new ReceiptLine(line.label, line.cost));
                    }
                }

                database.Nodes.Add(node);
            }

            database.BuildCache();
        }

        private RouteType ParseRouteType(string type)
        {
            return type?.ToLower() switch
            {
                "incel" => RouteType.Incel,
                "femcel" => RouteType.Femcel,
                "performative" => RouteType.Performative,
                "bop" => RouteType.Bop,
                "themcel" => RouteType.Themcel,
                _ => RouteType.None
            };
        }
    }

    // JSON data structures for parsing
    [System.Serializable]
    public class DialogueRouteData
    {
        public string routeName;
        public string routeType;
        public DialogueNodeData[] nodes;
    }

    [System.Serializable]
    public class DialogueNodeData
    {
        public string id;
        public string speaker;
        public string text;
        public string dateExpression;
        public string playerExpression;
        public bool isEnding;
        public string endingTitle;
        public string endingText;
        public DialogueChoiceData[] choices;
        public ReceiptLineData[] endingReceiptLines;
    }

    [System.Serializable]
    public class DialogueChoiceData
    {
        public string label;
        public string nextId;
        public ChoiceEffectsData effects;
        public ReceiptLineData[] receiptLines;
    }

    [System.Serializable]
    public class ChoiceEffectsData
    {
        public int moneyChange;
        public int patienceChange;
        public string[] tags;
    }

    [System.Serializable]
    public class ReceiptLineData
    {
        public string label;
        public int cost;
    }
}
