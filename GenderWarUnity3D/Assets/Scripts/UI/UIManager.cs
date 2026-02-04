using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Manages all UI screens and transitions
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Screens")]
    public GameObject TitleScreen;
    public GameObject CharacterCreatorScreen;
    public GameObject DateSelectionScreen;
    public GameObject GameScreen;
    public GameObject EndingScreen;
    public GameObject PauseMenu;
    public GameObject PhoneOverlay;

    [Header("HUD Elements")]
    public TextMeshProUGUI MoneyText;
    public TextMeshProUGUI PatienceText;
    public Image MoneyIcon;
    public Image PatienceIcon;

    [Header("Dialogue UI")]
    public DialogueUIController DialogueUI;

    [Header("Ending UI")]
    public EndingUIController EndingUI;

    [Header("Character Creator")]
    public CharacterCreatorUI CharacterCreator;

    [Header("Date Selection")]
    public DateSelectionUI DateSelection;

    [Header("Phone")]
    public PhoneUI Phone;

    [Header("Audio")]
    public AudioSource UIAudioSource;
    public AudioClip ButtonClickSound;
    public AudioClip ScreenTransitionSound;

    private GameObject currentScreen;

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

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
            GameManager.Instance.OnPatienceChanged += UpdatePatienceDisplay;
        }

        ShowScreen(TitleScreen);
    }

    public void ShowScreen(GameObject screen)
    {
        TitleScreen?.SetActive(false);
        CharacterCreatorScreen?.SetActive(false);
        DateSelectionScreen?.SetActive(false);
        GameScreen?.SetActive(false);
        EndingScreen?.SetActive(false);
        PauseMenu?.SetActive(false);

        if (screen != null)
        {
            screen.SetActive(true);
            currentScreen = screen;
            PlaySound(ScreenTransitionSound);
        }
    }

    public void ShowTitleScreen() => ShowScreen(TitleScreen);
    public void ShowCharacterCreator() => ShowScreen(CharacterCreatorScreen);
    public void ShowDateSelection(int part)
    {
        ShowScreen(DateSelectionScreen);
        DateSelection?.SetupForPart(part);
    }
    public void ShowGameScreen() => ShowScreen(GameScreen);

    public void ShowEndingScreen(DialogueNode endingNode)
    {
        ShowScreen(EndingScreen);
        EndingUI?.DisplayEnding(endingNode);
    }

    public void TogglePauseMenu()
    {
        if (PauseMenu != null)
        {
            bool isActive = PauseMenu.activeSelf;
            PauseMenu.SetActive(!isActive);
            Time.timeScale = isActive ? 1f : 0f;
        }
    }

    public void ShowPhone()
    {
        if (PhoneOverlay != null)
        {
            PhoneOverlay.SetActive(true);
            Phone?.DisplayCurrentDateProfile();
        }
    }

    public void HidePhone()
    {
        PhoneOverlay?.SetActive(false);
    }

    private void UpdateMoneyDisplay(int money)
    {
        if (MoneyText != null)
        {
            MoneyText.text = $"${money}";
            MoneyText.color = money < 0 ? Color.red : (money < 20 ? Color.yellow : Color.white);
        }
    }

    private void UpdatePatienceDisplay(int patience)
    {
        if (PatienceText != null)
        {
            PatienceText.text = patience.ToString();
            PatienceText.color = patience <= 2 ? Color.red : (patience <= 5 ? Color.yellow : Color.white);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (UIAudioSource != null && clip != null)
        {
            UIAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayButtonSound()
    {
        PlaySound(ButtonClickSound);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnMoneyChanged -= UpdateMoneyDisplay;
            GameManager.Instance.OnPatienceChanged -= UpdatePatienceDisplay;
        }
    }
}
