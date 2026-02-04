using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using GenderWar.Core;

namespace GenderWar.UI
{
    /// <summary>
    /// Controls the ending screen display including receipt
    /// </summary>
    public class EndingUIController : MonoBehaviour
    {
        [Header("Ending Display")]
        public TextMeshProUGUI EndingTitleText;
        public TextMeshProUGUI EndingDescriptionText;
        public Image EndingImage;

        [Header("Stats Display")]
        public TextMeshProUGUI FinalMoneyText;
        public TextMeshProUGUI FinalPatienceText;

        [Header("Receipt")]
        public GameObject ReceiptPanel;
        public Transform ReceiptItemContainer;
        public GameObject ReceiptItemPrefab;
        public TextMeshProUGUI ReceiptTotalText;
        public TextMeshProUGUI ReceiptBalanceText;
        public TextMeshProUGUI ReceiptMessageText;

        [Header("Action Buttons")]
        public Button PlayAgainButton;
        public Button DifferentDateButton;
        public Button NewCharacterButton;
        public Button NextDateButton;
        public Button FinalPhaseButton;
        public Button MainMenuButton;

        [Header("Animation")]
        public Animator EndingAnimator;

        private List<GameObject> receiptItems = new List<GameObject>();

        private void Start()
        {
            // Setup button listeners
            PlayAgainButton?.onClick.AddListener(OnPlayAgain);
            DifferentDateButton?.onClick.AddListener(OnDifferentDate);
            NewCharacterButton?.onClick.AddListener(OnNewCharacter);
            NextDateButton?.onClick.AddListener(OnNextDate);
            FinalPhaseButton?.onClick.AddListener(OnFinalPhase);
            MainMenuButton?.onClick.AddListener(OnMainMenu);
        }

        public void DisplayEnding(DialogueNode endingNode)
        {
            // Display ending title and text
            if (EndingTitleText != null)
            {
                EndingTitleText.text = endingNode.EndingTitle ?? "THE END";
            }

            if (EndingDescriptionText != null)
            {
                EndingDescriptionText.text = endingNode.EndingText ?? "";
            }

            // Display final stats
            var state = GameManager.Instance.CurrentState;

            if (FinalMoneyText != null)
            {
                FinalMoneyText.text = $"${state.Money}";
                FinalMoneyText.color = state.Money < 0 ? Color.red : Color.green;
            }

            if (FinalPatienceText != null)
            {
                FinalPatienceText.text = state.Patience.ToString();
            }

            // Build receipt
            BuildReceipt(state);

            // Setup buttons based on current part
            SetupButtons(state);

            // Play animation
            EndingAnimator?.SetTrigger("ShowEnding");
        }

        private void BuildReceipt(GameState state)
        {
            // Clear existing items
            foreach (var item in receiptItems)
            {
                Destroy(item);
            }
            receiptItems.Clear();

            if (ReceiptItemPrefab == null || ReceiptItemContainer == null) return;

            int total = 0;

            // Add receipt lines
            foreach (var line in state.Receipt)
            {
                var item = Instantiate(ReceiptItemPrefab, ReceiptItemContainer);
                receiptItems.Add(item);

                var labelText = item.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();
                var costText = item.transform.Find("Cost")?.GetComponent<TextMeshProUGUI>();

                if (labelText != null)
                {
                    labelText.text = line.Label;
                }

                if (costText != null)
                {
                    if (line.Cost > 0)
                    {
                        costText.text = $"${line.Cost}";
                        total += line.Cost;
                    }
                    else
                    {
                        costText.text = "---";
                    }
                }
            }

            // Update totals
            if (ReceiptTotalText != null)
            {
                ReceiptTotalText.text = $"TOTAL: ${total}";
            }

            int balance = 100 - total;
            if (ReceiptBalanceText != null)
            {
                ReceiptBalanceText.text = $"BALANCE: ${balance}";
                ReceiptBalanceText.color = balance < 0 ? Color.red : Color.green;
            }

            // Receipt message
            if (ReceiptMessageText != null)
            {
                ReceiptMessageText.text = GetReceiptMessage(balance);
            }
        }

        private string GetReceiptMessage(int balance)
        {
            if (balance < -50) return "Your card was declined. Twice.";
            if (balance < 0) return "Your card was declined.";
            if (balance < 10) return "Hope you weren't planning to tip.";
            if (balance < 30) return "The waiter is judging you.";
            if (balance < 60) return "Adequate.";
            if (balance < 90) return "You kept your wallet mostly intact.";
            return "Financial responsibility achieved.";
        }

        private void SetupButtons(GameState state)
        {
            // Hide all buttons first
            PlayAgainButton?.gameObject.SetActive(true);
            DifferentDateButton?.gameObject.SetActive(false);
            NewCharacterButton?.gameObject.SetActive(true);
            NextDateButton?.gameObject.SetActive(false);
            FinalPhaseButton?.gameObject.SetActive(false);
            MainMenuButton?.gameObject.SetActive(true);

            switch (state.CurrentPart)
            {
                case 1:
                    // Part 1 endings show "Different Date" and "Next Date" (for Part 2)
                    DifferentDateButton?.gameObject.SetActive(true);
                    NextDateButton?.gameObject.SetActive(state.Part2Unlocked);
                    break;

                case 2:
                    // Part 2 endings show "Final Phase" button
                    FinalPhaseButton?.gameObject.SetActive(state.Part3Unlocked);
                    break;

                case 3:
                    // Part 3 - just show main menu and play again
                    break;
            }
        }

        private void OnPlayAgain()
        {
            UIManager.Instance?.PlayButtonSound();
            GameManager.Instance?.RestartCurrentDate();
            UIManager.Instance?.ShowGameScreen();
        }

        private void OnDifferentDate()
        {
            UIManager.Instance?.PlayButtonSound();
            UIManager.Instance?.ShowDateSelection(1);
        }

        private void OnNewCharacter()
        {
            UIManager.Instance?.PlayButtonSound();
            GameManager.Instance?.StartNewGame();
            UIManager.Instance?.ShowCharacterCreator();
        }

        private void OnNextDate()
        {
            UIManager.Instance?.PlayButtonSound();
            UIManager.Instance?.ShowDateSelection(2);
        }

        private void OnFinalPhase()
        {
            UIManager.Instance?.PlayButtonSound();
            GameManager.Instance?.StartDate(RouteType.Themcel, 3);
            UIManager.Instance?.ShowGameScreen();
        }

        private void OnMainMenu()
        {
            UIManager.Instance?.PlayButtonSound();
            GameManager.Instance?.ReturnToMainMenu();
        }
    }
}
