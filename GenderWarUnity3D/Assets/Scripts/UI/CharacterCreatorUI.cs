using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GenderWar.Core;

namespace GenderWar.UI
{
    /// <summary>
    /// Controls the character creator interface - "BUILD YOUR PLAYBOY BUNNY"
    /// </summary>
    public class CharacterCreatorUI : MonoBehaviour
    {
        [Header("Preview")]
        public Image PreviewImage;
        public RawImage Preview3DRenderTexture;
        public Camera PreviewCamera;
        public Transform PreviewCharacterSpawn;

        [Header("Skin Tone Selection")]
        public Button[] SkinToneButtons;
        public Color[] SkinToneColors;

        [Header("Eye Selection")]
        public Button[] EyeButtons;
        public string[] EyeSymbols = { "◕◕", "◉◉", "≧≦", "♥♥" };

        [Header("Mouth Selection")]
        public Button[] MouthButtons;
        public string[] MouthSymbols = { "ω", "︿", "○", "з" };

        [Header("Accessory Selection")]
        public Button[] AccessoryButtons;
        public string[] AccessorySymbols = { "🎀", "🎩", "🌸", "⭐" };
        public Sprite[] AccessorySprites;

        [Header("Preview Character")]
        public GameObject PreviewCharacterPrefab;

        [Header("Buttons")]
        public Button BackButton;
        public Button ConfirmButton;

        [Header("Selection Indicators")]
        public Color SelectedColor = new Color(1f, 0.3f, 0.42f);
        public Color UnselectedColor = Color.white;

        private PlayerAppearance currentAppearance;
        private GameObject previewCharacter;

        private void OnEnable()
        {
            currentAppearance = new PlayerAppearance();
            SetupButtons();
            UpdatePreview();
        }

        private void SetupButtons()
        {
            // Skin tone buttons
            for (int i = 0; i < SkinToneButtons.Length; i++)
            {
                int index = i;
                SkinToneButtons[i].onClick.AddListener(() => SelectSkinTone(index));

                // Set button color to represent the skin tone
                if (i < SkinToneColors.Length)
                {
                    var colors = SkinToneButtons[i].colors;
                    colors.normalColor = SkinToneColors[i];
                    SkinToneButtons[i].colors = colors;
                }
            }

            // Eye buttons
            for (int i = 0; i < EyeButtons.Length; i++)
            {
                int index = i;
                EyeButtons[i].onClick.AddListener(() => SelectEyes(index));

                // Set button text
                var text = EyeButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && i < EyeSymbols.Length)
                {
                    text.text = EyeSymbols[i];
                }
            }

            // Mouth buttons
            for (int i = 0; i < MouthButtons.Length; i++)
            {
                int index = i;
                MouthButtons[i].onClick.AddListener(() => SelectMouth(index));

                var text = MouthButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && i < MouthSymbols.Length)
                {
                    text.text = MouthSymbols[i];
                }
            }

            // Accessory buttons
            for (int i = 0; i < AccessoryButtons.Length; i++)
            {
                int index = i;
                AccessoryButtons[i].onClick.AddListener(() => SelectAccessory(index));

                var text = AccessoryButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && i < AccessorySymbols.Length)
                {
                    text.text = AccessorySymbols[i];
                }
            }

            // Navigation buttons
            BackButton?.onClick.AddListener(OnBack);
            ConfirmButton?.onClick.AddListener(OnConfirm);
        }

        private void SelectSkinTone(int index)
        {
            UIManager.Instance?.PlayButtonSound();
            currentAppearance.SkinTone = index;
            UpdateSelectionIndicators(SkinToneButtons, index);
            UpdatePreview();
        }

        private void SelectEyes(int index)
        {
            UIManager.Instance?.PlayButtonSound();
            currentAppearance.EyeStyle = index;
            UpdateSelectionIndicators(EyeButtons, index);
            UpdatePreview();
        }

        private void SelectMouth(int index)
        {
            UIManager.Instance?.PlayButtonSound();
            currentAppearance.MouthStyle = index;
            UpdateSelectionIndicators(MouthButtons, index);
            UpdatePreview();
        }

        private void SelectAccessory(int index)
        {
            UIManager.Instance?.PlayButtonSound();
            currentAppearance.Accessory = index;
            UpdateSelectionIndicators(AccessoryButtons, index);
            UpdatePreview();
        }

        private void UpdateSelectionIndicators(Button[] buttons, int selectedIndex)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                var outline = buttons[i].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = (i == selectedIndex);
                    outline.effectColor = SelectedColor;
                }

                // Alternative: scale selected button
                buttons[i].transform.localScale = (i == selectedIndex)
                    ? Vector3.one * 1.1f
                    : Vector3.one;
            }
        }

        private void UpdatePreview()
        {
            // Update 3D preview character
            if (previewCharacter != null)
            {
                var display = previewCharacter.GetComponent<Characters.Character3DDisplay>();
                display?.ApplyPlayerAppearance(currentAppearance);
            }
            else if (PreviewCharacterPrefab != null && PreviewCharacterSpawn != null)
            {
                previewCharacter = Instantiate(PreviewCharacterPrefab, PreviewCharacterSpawn);
                var display = previewCharacter.GetComponent<Characters.Character3DDisplay>();
                display?.ApplyPlayerAppearance(currentAppearance);
            }

            // Update 2D preview if using sprite-based display
            UpdateSpritePreview();
        }

        private void UpdateSpritePreview()
        {
            // This would update a 2D sprite preview
            // The sprite would be generated based on the appearance settings
            // For now, we'll rely on the 3D preview
        }

        private void OnBack()
        {
            UIManager.Instance?.PlayButtonSound();
            UIManager.Instance?.ShowTitleScreen();
        }

        private void OnConfirm()
        {
            UIManager.Instance?.PlayButtonSound();

            // Save appearance to game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentState.Appearance = currentAppearance;
            }

            // Proceed to date selection
            UIManager.Instance?.ShowDateSelection(1);
        }

        private void OnDisable()
        {
            // Cleanup preview character
            if (previewCharacter != null)
            {
                Destroy(previewCharacter);
                previewCharacter = null;
            }
        }
    }
}
