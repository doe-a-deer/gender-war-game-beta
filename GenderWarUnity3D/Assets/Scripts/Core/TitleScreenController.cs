using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Controls the title screen with video background
/// </summary>
public class TitleScreenController : MonoBehaviour
{
    [Header("Title Elements")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI SubtitleText;
    public Button StartButton;
    public Button CreditsButton;
    public Button QuitButton;

    [Header("Video Background")]
    public VideoPlayer BackgroundVideo;
    public RawImage VideoDisplay;
    public RenderTexture VideoRenderTexture;

    [Header("Audio")]
    public AudioSource TitleMusic;
    public AudioClip TitleTrack;

    [Header("Animation")]
    public Animator TitleAnimator;
    public float FadeInDuration = 1f;
    public CanvasGroup TitleCanvasGroup;

    private void Start()
    {
        SetupTitleScreen();
        SetupButtons();
        PlayTitleSequence();
    }

    private void SetupTitleScreen()
    {
        if (TitleText != null)
            TitleText.text = "GENDER WAR";

        if (SubtitleText != null)
            SubtitleText.text = "A Dating Disaster Simulator";

        if (BackgroundVideo != null && VideoRenderTexture != null)
        {
            BackgroundVideo.targetTexture = VideoRenderTexture;
            BackgroundVideo.isLooping = true;
            BackgroundVideo.Play();
        }

        if (TitleMusic != null && TitleTrack != null)
        {
            TitleMusic.clip = TitleTrack;
            TitleMusic.loop = true;
            TitleMusic.Play();
        }
    }

    private void SetupButtons()
    {
        StartButton?.onClick.AddListener(OnStartClicked);
        CreditsButton?.onClick.AddListener(OnCreditsClicked);
        QuitButton?.onClick.AddListener(OnQuitClicked);
    }

    private void PlayTitleSequence()
    {
        if (TitleCanvasGroup != null)
        {
            TitleCanvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }
        TitleAnimator?.SetTrigger("PlayIntro");
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < FadeInDuration)
        {
            elapsed += Time.deltaTime;
            TitleCanvasGroup.alpha = elapsed / FadeInDuration;
            yield return null;
        }
        TitleCanvasGroup.alpha = 1f;
    }

    private void OnStartClicked()
    {
        UIManager.Instance?.PlayButtonSound();

        if (TitleMusic != null)
            StartCoroutine(FadeOutMusic());

        GameManager.Instance?.StartNewGame();
        UIManager.Instance?.ShowCharacterCreator();
    }

    private System.Collections.IEnumerator FadeOutMusic()
    {
        float startVolume = TitleMusic.volume;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            TitleMusic.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }
        TitleMusic.Stop();
    }

    private void OnCreditsClicked()
    {
        UIManager.Instance?.PlayButtonSound();
        Debug.Log("Credits clicked - implement credits panel");
    }

    private void OnQuitClicked()
    {
        UIManager.Instance?.PlayButtonSound();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (BackgroundVideo != null)
            BackgroundVideo.Stop();
    }
}
