using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public string[] EyeSymbols = { "eyes1", "eyes2", "eyes3", "eyes4" };

    [Header("Mouth Selection")]
    public Button[] MouthButtons;
    public string[] MouthSymbols = { "mouth1", "mouth2", "mouth3", "mouth4" };

    [Header("Accessory Selection")]
    public Button[] AccessoryButtons;
    public string[] AccessorySymbols = { "bow", "hat", "flower", "star" };
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
        for (int i = 0; i < SkinToneButtons.Length; i++)
        {
            int index = i;
            SkinToneButtons[i].onClick.AddListener(() => SelectSkinTone(index));
            if (i < SkinToneColors.Length)
            {
                var colors = SkinToneButtons[i].colors;
                colors.normalColor = SkinToneColors[i];
                SkinToneButtons[i].colors = colors;
            }
        }

        for (int i = 0; i < EyeButtons.Length; i++)
        {
            int index = i;
            EyeButtons[i].onClick.AddListener(() => SelectEyes(index));
        }

        for (int i = 0; i < MouthButtons.Length; i++)
        {
            int index = i;
            MouthButtons[i].onClick.AddListener(() => SelectMouth(index));
        }

        for (int i = 0; i < AccessoryButtons.Length; i++)
        {
            int index = i;
            AccessoryButtons[i].onClick.AddListener(() => SelectAccessory(index));
        }

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
            buttons[i].transform.localScale = (i == selectedIndex) ? Vector3.one * 1.1f : Vector3.one;
        }
    }

    private void UpdatePreview()
    {
        if (previewCharacter != null)
        {
            var display = previewCharacter.GetComponent<Character3DDisplay>();
            display?.ApplyPlayerAppearance(currentAppearance);
        }
        else if (PreviewCharacterPrefab != null && PreviewCharacterSpawn != null)
        {
            previewCharacter = Instantiate(PreviewCharacterPrefab, PreviewCharacterSpawn);
            var display = previewCharacter.GetComponent<Character3DDisplay>();
            display?.ApplyPlayerAppearance(currentAppearance);
        }
    }

    private void OnBack()
    {
        UIManager.Instance?.PlayButtonSound();
        UIManager.Instance?.ShowTitleScreen();
    }

    private void OnConfirm()
    {
        UIManager.Instance?.PlayButtonSound();
        if (GameManager.Instance != null)
            GameManager.Instance.CurrentState.Appearance = currentAppearance;
        UIManager.Instance?.ShowDateSelection(1);
    }

    private void OnDisable()
    {
        if (previewCharacter != null)
        {
            Destroy(previewCharacter);
            previewCharacter = null;
        }
    }
}
