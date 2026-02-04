using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GenderWar.Core;
using GenderWar.Characters;

namespace GenderWar.UI
{
    /// <summary>
    /// Controls the date selection screen
    /// </summary>
    public class DateSelectionUI : MonoBehaviour
    {
        [Header("Part 1 Characters")]
        public GameObject Part1Container;
        public DateCard IncelCard;
        public DateCard FemcelCard;

        [Header("Part 2 Characters")]
        public GameObject Part2Container;
        public DateCard PerformativeCard;
        public DateCard BopCard;

        [Header("Character Data")]
        public CharacterData IncelData;
        public CharacterData FemcelData;
        public CharacterData PerformativeData;
        public CharacterData BopData;

        [Header("Header")]
        public TextMeshProUGUI HeaderText;
        public TextMeshProUGUI SubheaderText;

        [Header("Navigation")]
        public Button BackButton;

        private int currentPart = 1;

        private void Start()
        {
            // Setup card click handlers
            IncelCard?.SetClickHandler(() => SelectDate(RouteType.Incel));
            FemcelCard?.SetClickHandler(() => SelectDate(RouteType.Femcel));
            PerformativeCard?.SetClickHandler(() => SelectDate(RouteType.Performative));
            BopCard?.SetClickHandler(() => SelectDate(RouteType.Bop));

            // Setup navigation
            BackButton?.onClick.AddListener(OnBack);
        }

        public void SetupForPart(int part)
        {
            currentPart = part;

            // Update header
            if (HeaderText != null)
            {
                HeaderText.text = part == 1 ? "CHOOSE YOUR DATE" : "CHOOSE YOUR NEXT DATE";
            }

            if (SubheaderText != null)
            {
                SubheaderText.text = part == 1
                    ? "Pick your poison"
                    : "The rumors have spread...";
            }

            // Show appropriate characters
            Part1Container?.SetActive(part == 1);
            Part2Container?.SetActive(part == 2);

            // Setup cards with character data
            if (part == 1)
            {
                IncelCard?.SetupCard(IncelData);
                FemcelCard?.SetupCard(FemcelData);
            }
            else
            {
                PerformativeCard?.SetupCard(PerformativeData);
                BopCard?.SetupCard(BopData);
            }
        }

        private void SelectDate(RouteType route)
        {
            UIManager.Instance?.PlayButtonSound();
            GameManager.Instance?.StartDate(route, currentPart);
            UIManager.Instance?.ShowGameScreen();
        }

        private void OnBack()
        {
            UIManager.Instance?.PlayButtonSound();

            if (currentPart == 1)
            {
                UIManager.Instance?.ShowCharacterCreator();
            }
            else
            {
                // Return to ending screen or main menu
                UIManager.Instance?.ShowTitleScreen();
            }
        }
    }

    /// <summary>
    /// Individual date card component
    /// </summary>
    [System.Serializable]
    public class DateCard : MonoBehaviour
    {
        [Header("Card Elements")]
        public Image CharacterImage;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI AgeText;
        public TextMeshProUGUI BioText;
        public Transform TagContainer;
        public GameObject TagPrefab;

        [Header("Visual")]
        public Image CardBackground;
        public Button CardButton;

        private System.Action clickHandler;

        public void SetupCard(CharacterData data)
        {
            if (data == null) return;

            // Set portrait
            if (CharacterImage != null)
            {
                CharacterImage.sprite = data.PortraitNeutral;
            }

            // Set text
            if (NameText != null)
            {
                NameText.text = data.CharacterName;
            }

            if (AgeText != null)
            {
                AgeText.text = $"{data.Age}";
            }

            if (BioText != null && data.Profile != null)
            {
                BioText.text = string.Join("\n", data.Profile.BioLines);
            }

            // Setup tags
            SetupTags(data.Profile?.Tags);

            // Set card color
            if (CardBackground != null)
            {
                CardBackground.color = data.CharacterColor;
            }
        }

        private void SetupTags(string[] tags)
        {
            if (TagContainer == null || TagPrefab == null || tags == null) return;

            // Clear existing tags
            foreach (Transform child in TagContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new tags
            foreach (var tag in tags)
            {
                var tagObj = Instantiate(TagPrefab, TagContainer);
                var text = tagObj.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = tag;
                }
            }
        }

        public void SetClickHandler(System.Action handler)
        {
            clickHandler = handler;
            CardButton?.onClick.AddListener(() => clickHandler?.Invoke());
        }
    }
}
