using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the dialogue box, speaker names, and choice buttons
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("Dialogue Box")]
    public GameObject DialoguePanel;
    public TextMeshProUGUI SpeakerNameText;
    public TextMeshProUGUI DialogueText;
    public Image SpeakerPortrait;

    [Header("Choice Buttons")]
    public Transform ChoiceContainer;
    public GameObject ChoiceButtonPrefab;
    public int MaxChoices = 3;

    [Header("Character Display")]
    public Image DateCharacterImage;
    public Image PlayerCharacterImage;

    [Header("Typewriter Effect")]
    public bool UseTypewriter = true;
    public float TypewriterSpeed = 0.03f;
    public AudioSource TypewriterAudio;
    public AudioClip TypewriterSound;

    [Header("Animation")]
    public Animator DialoguePanelAnimator;

    private List<GameObject> choiceButtons = new List<GameObject>();
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string fullText;

    private void Start()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnNodeChanged += DisplayNode;
        CreateChoiceButtons();
    }

    private void CreateChoiceButtons()
    {
        for (int i = 0; i < MaxChoices; i++)
        {
            if (ChoiceButtonPrefab != null && ChoiceContainer != null)
            {
                var button = Instantiate(ChoiceButtonPrefab, ChoiceContainer);
                button.SetActive(false);
                choiceButtons.Add(button);
                int index = i;
                button.GetComponent<Button>()?.onClick.AddListener(() => OnChoiceClicked(index));
            }
        }
    }

    public void DisplayNode(DialogueNode node)
    {
        if (node == null) return;

        if (SpeakerNameText != null)
            SpeakerNameText.text = FormatSpeakerName(node.Speaker);

        UpdateCharacterDisplay(node);

        if (UseTypewriter)
            StartTypewriter(node.Text);
        else if (DialogueText != null)
            DialogueText.text = node.Text;

        if (!UseTypewriter)
            DisplayChoices(node.Choices);
        else
            HideAllChoices();

        DialoguePanelAnimator?.SetTrigger("NewDialogue");
    }

    private void StartTypewriter(string text)
    {
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);
        fullText = text;
        typewriterCoroutine = StartCoroutine(TypewriterEffect(text));
    }

    private IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;
        if (DialogueText != null) DialogueText.text = "";

        foreach (char c in text)
        {
            if (DialogueText != null) DialogueText.text += c;
            if (!char.IsWhiteSpace(c) && TypewriterAudio != null && TypewriterSound != null)
            {
                TypewriterAudio.pitch = Random.Range(0.9f, 1.1f);
                TypewriterAudio.PlayOneShot(TypewriterSound, 0.3f);
            }
            yield return new WaitForSeconds(TypewriterSpeed);
        }

        isTyping = false;
        var currentNode = DialogueManager.Instance?.GetCurrentNode();
        if (currentNode != null) DisplayChoices(currentNode.Choices);
    }

    public void SkipTypewriter()
    {
        if (isTyping && typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            if (DialogueText != null) DialogueText.text = fullText;
            isTyping = false;
            var currentNode = DialogueManager.Instance?.GetCurrentNode();
            if (currentNode != null) DisplayChoices(currentNode.Choices);
        }
    }

    private void Update()
    {
        if (isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            SkipTypewriter();
    }

    private void DisplayChoices(List<DialogueChoice> choices)
    {
        HideAllChoices();
        if (choices == null || choices.Count == 0) return;

        for (int i = 0; i < choices.Count && i < choiceButtons.Count; i++)
        {
            var button = choiceButtons[i];
            button.SetActive(true);
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = choices[i].Label;
        }
    }

    private void HideAllChoices()
    {
        foreach (var button in choiceButtons)
            button.SetActive(false);
    }

    private void OnChoiceClicked(int index)
    {
        UIManager.Instance?.PlayButtonSound();
        DialogueManager.Instance?.MakeChoice(index);
    }

    private void UpdateCharacterDisplay(DialogueNode node)
    {
        if (DateCharacterImage != null && CharacterManager.Instance != null)
        {
            var dateChar = CharacterManager.Instance.GetCurrentDateCharacter();
            if (dateChar != null)
                DateCharacterImage.sprite = dateChar.GetExpression(node.DateExpression);
        }
        CharacterManager.Instance?.SetDateExpression(node.DateExpression);
        CharacterManager.Instance?.SetPlayerExpression(node.PlayerExpression);
    }

    private string FormatSpeakerName(string speaker)
    {
        if (string.IsNullOrEmpty(speaker)) return "";
        return speaker.ToUpper() switch
        {
            "INCEL" => "KEVIN",
            "FEMCEL" => "JESSICA",
            "PERFORMATIVE" => "BRENDAN",
            "BOP" => "MADISON",
            "WAITER" => "WAITER",
            "PLAYER" => "YOU",
            "THEM" => "THE GROUP",
            _ => speaker
        };
    }

    private void OnDestroy()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnNodeChanged -= DisplayNode;
    }
}
