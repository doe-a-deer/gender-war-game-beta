using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

namespace GenderWar.Core
{
    /// <summary>
    /// Main game manager - handles game state, progression, and core systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState CurrentState;

        [Header("References")]
        public DialogueManager DialogueManager;
        public UIManager UIManager;
        public CharacterManager CharacterManager;
        public ReputationSystem ReputationSystem;
        public IntegrationSystem IntegrationSystem;

        // Events
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnMoneyChanged;
        public event Action<int> OnPatienceChanged;
        public event Action OnGameEnded;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGame()
        {
            CurrentState = new GameState();
            ReputationSystem = new ReputationSystem();
            IntegrationSystem = new IntegrationSystem();
        }

        public void StartNewGame()
        {
            CurrentState = new GameState();
            ReputationSystem.Reset();
            IntegrationSystem.Reset();
            OnGameStateChanged?.Invoke(CurrentState);
        }

        public void StartDate(RouteType route, int part)
        {
            CurrentState.CurrentRoute = route;
            CurrentState.CurrentPart = part;
            CurrentState.Money = 100;
            CurrentState.Patience = 10;
            CurrentState.Receipt.Clear();
            CurrentState.CurrentNodeId = "start";

            OnGameStateChanged?.Invoke(CurrentState);
            OnMoneyChanged?.Invoke(CurrentState.Money);
            OnPatienceChanged?.Invoke(CurrentState.Patience);

            DialogueManager?.StartDialogue(route);
        }

        public void ModifyMoney(int amount)
        {
            CurrentState.Money += amount;
            OnMoneyChanged?.Invoke(CurrentState.Money);
        }

        public void ModifyPatience(int amount)
        {
            CurrentState.Patience += amount;
            OnPatienceChanged?.Invoke(CurrentState.Patience);

            if (CurrentState.Patience <= 0)
            {
                TriggerPatienceEnding();
            }
        }

        public void AddReceiptLine(string label, int cost)
        {
            CurrentState.Receipt.Add(new ReceiptLine(label, cost));
        }

        public void LogChoice(string nodeId, string choiceLabel, ChoiceEffects effects)
        {
            var logEntry = new ChoiceLogEntry(nodeId, choiceLabel, effects);
            CurrentState.RunLog.Add(logEntry);
            ReputationSystem.ProcessChoice(logEntry);
        }

        public void UnlockPart(int part)
        {
            if (part == 2) CurrentState.Part2Unlocked = true;
            if (part == 3) CurrentState.Part3Unlocked = true;
        }

        public void EndDate(DialogueNode endingNode)
        {
            CurrentState.IsEnded = true;
            UnlockPart(CurrentState.CurrentPart + 1);
            OnGameEnded?.Invoke();
            UIManager?.ShowEndingScreen(endingNode);
        }

        private void TriggerPatienceEnding()
        {
            // Patience ran out - trigger special ending
            var patienceEnding = new DialogueNode
            {
                IsEnding = true,
                EndingTitle = "PATIENCE EXHAUSTED",
                EndingText = "You've reached your limit. Some dates just aren't worth the effort."
            };
            EndDate(patienceEnding);
        }

        public string GetRumorLine()
        {
            return ReputationSystem.GenerateRumor();
        }

        public IntegrationResult CalculateIntegration()
        {
            return IntegrationSystem.Calculate(ReputationSystem.GetTags());
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void RestartCurrentDate()
        {
            StartDate(CurrentState.CurrentRoute, CurrentState.CurrentPart);
        }
    }

    [Serializable]
    public class GameState
    {
        public PlayerAppearance Appearance = new PlayerAppearance();
        public RouteType CurrentRoute = RouteType.None;
        public int CurrentPart = 1;
        public string CurrentNodeId = "start";
        public int Money = 100;
        public int Patience = 10;
        public List<ReceiptLine> Receipt = new List<ReceiptLine>();
        public List<ChoiceLogEntry> RunLog = new List<ChoiceLogEntry>();
        public bool Part2Unlocked = false;
        public bool Part3Unlocked = false;
        public bool IsEnded = false;
    }

    [Serializable]
    public class PlayerAppearance
    {
        public int SkinTone = 0;
        public int EyeStyle = 0;
        public int MouthStyle = 0;
        public int Accessory = 0;
    }

    [Serializable]
    public class ReceiptLine
    {
        public string Label;
        public int Cost;

        public ReceiptLine(string label, int cost)
        {
            Label = label;
            Cost = cost;
        }
    }

    [Serializable]
    public class ChoiceLogEntry
    {
        public string NodeId;
        public string ChoiceLabel;
        public ChoiceEffects Effects;
        public DateTime Timestamp;

        public ChoiceLogEntry(string nodeId, string label, ChoiceEffects effects)
        {
            NodeId = nodeId;
            ChoiceLabel = label;
            Effects = effects;
            Timestamp = DateTime.Now;
        }
    }

    [Serializable]
    public class ChoiceEffects
    {
        public int MoneyChange = 0;
        public int PatienceChange = 0;
        public List<string> Tags = new List<string>();
    }

    public enum RouteType
    {
        None,
        Incel,
        Femcel,
        Performative,
        Bop,
        Themcel
    }
}
