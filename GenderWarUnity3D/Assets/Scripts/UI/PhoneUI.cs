using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the phone overlay - DisasterMatch dating app profile view
/// </summary>
public class PhoneUI : MonoBehaviour
{
    [Header("Phone Frame")]
    public GameObject PhoneFrame;
    public Button CloseButton;
    public Button PhoneToggleButton;

    [Header("App Header")]
    public TextMeshProUGUI AppNameText;
    public Image AppLogo;

    [Header("Profile Content")]
    public Image ProfilePhoto;
    public TextMeshProUGUI ProfileName;
    public TextMeshProUGUI ProfileAge;
    public TextMeshProUGUI ProfileBio;

    [Header("Profile Tags")]
    public Transform TagContainer;
    public GameObject TagPrefab;

    [Header("Match Info")]
    public TextMeshProUGUI MatchPercentText;
    public TextMeshProUGUI DistanceText;

    [Header("Animation")]
    public Animator PhoneAnimator;

    [Header("Audio")]
    public AudioSource PhoneAudio;
    public AudioClip PhoneOpenSound;
    public AudioClip PhoneCloseSound;
    public AudioClip NotificationSound;

    private bool isOpen = false;

    private void Start()
    {
        CloseButton?.onClick.AddListener(ClosePhone);
        PhoneToggleButton?.onClick.AddListener(TogglePhone);

        if (AppNameText != null)
            AppNameText.text = "DisasterMatch";
    }

    public void TogglePhone()
    {
        if (isOpen) ClosePhone();
        else OpenPhone();
    }

    public void OpenPhone()
    {
        PhoneFrame?.SetActive(true);
        isOpen = true;
        DisplayCurrentDateProfile();
        PhoneAnimator?.SetTrigger("Open");
        PlaySound(PhoneOpenSound);
    }

    public void ClosePhone()
    {
        PhoneAnimator?.SetTrigger("Close");
        PlaySound(PhoneCloseSound);
        isOpen = false;
        Invoke(nameof(HidePhone), 0.3f);
    }

    private void HidePhone()
    {
        PhoneFrame?.SetActive(false);
    }

    public void DisplayCurrentDateProfile()
    {
        var characterData = CharacterManager.Instance?.GetCurrentDateCharacter();
        if (characterData == null || characterData.Profile == null) return;

        var profile = characterData.Profile;

        if (ProfilePhoto != null)
            ProfilePhoto.sprite = profile.Photo ?? characterData.PortraitNeutral;

        if (ProfileName != null)
            ProfileName.text = profile.DisplayName ?? characterData.CharacterName;

        if (ProfileAge != null)
            ProfileAge.text = $"{profile.DisplayAge}";

        if (ProfileBio != null)
            ProfileBio.text = string.Join("\n", profile.BioLines ?? new string[0]);

        SetupTags(profile.Tags);

        if (MatchPercentText != null)
        {
            int matchPercent = Random.Range(1, 15);
            MatchPercentText.text = $"{matchPercent}% Match";
        }

        if (DistanceText != null)
            DistanceText.text = "0 miles away (unfortunately)";
    }

    private void SetupTags(string[] tags)
    {
        if (TagContainer == null || TagPrefab == null) return;

        foreach (Transform child in TagContainer)
            Destroy(child.gameObject);

        if (tags == null) return;

        foreach (var tag in tags)
        {
            var tagObj = Instantiate(TagPrefab, TagContainer);
            var text = tagObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = tag;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (PhoneAudio != null && clip != null)
            PhoneAudio.PlayOneShot(clip);
    }

    public void PlayNotification()
    {
        PlaySound(NotificationSound);
    }
}
