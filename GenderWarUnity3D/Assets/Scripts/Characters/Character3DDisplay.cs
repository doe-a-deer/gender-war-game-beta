using UnityEngine;

/// <summary>
/// Controls 3D character display, expressions, and animations
/// </summary>
public class Character3DDisplay : MonoBehaviour
{
    [Header("Components")]
    public MeshRenderer CharacterRenderer;
    public MeshFilter CharacterMesh;
    public Animator CharacterAnimator;

    [Header("Expression Display")]
    public SpriteRenderer ExpressionSprite;
    public Transform FacePosition;

    [Header("Player Customization")]
    public MeshRenderer SkinRenderer;
    public Transform AccessoryMount;
    public GameObject[] AccessoryPrefabs;

    private CharacterData characterData;
    private string currentExpression = "neutral";

    private readonly Color[] SkinTones = new Color[]
    {
        new Color(0.7f, 0.9f, 0.7f),
        new Color(0.9f, 0.8f, 0.7f),
        new Color(0.6f, 0.4f, 0.3f),
        new Color(1f, 0.7f, 0.8f),
        new Color(0.7f, 0.8f, 1f)
    };

    public void Initialize(CharacterData data)
    {
        characterData = data;
        if (data == null) return;

        if (CharacterMesh != null && data.Character3DMesh != null)
            CharacterMesh.mesh = data.Character3DMesh;

        if (CharacterRenderer != null && data.Character3DMaterial != null)
            CharacterRenderer.material = data.Character3DMaterial;

        SetExpression("neutral");
    }

    public void SetExpression(string expression)
    {
        currentExpression = expression;
        if (characterData == null) return;

        if (ExpressionSprite != null)
            ExpressionSprite.sprite = characterData.GetExpression(expression);

        if (CharacterRenderer != null)
        {
            var mat = characterData.GetExpressionMaterial(expression);
            if (mat != null)
                CharacterRenderer.material = mat;
        }

        if (CharacterAnimator != null)
            CharacterAnimator.SetTrigger(expression);
    }

    public void ApplyPlayerAppearance(PlayerAppearance appearance)
    {
        if (SkinRenderer != null && appearance.SkinTone < SkinTones.Length)
        {
            var mat = SkinRenderer.material;
            mat.color = SkinTones[appearance.SkinTone];
        }

        ApplyAccessory(appearance.Accessory);
        ApplyFaceCustomization(appearance.EyeStyle, appearance.MouthStyle);
    }

    private void ApplyAccessory(int accessoryIndex)
    {
        if (AccessoryPrefabs != null)
        {
            foreach (var accessory in AccessoryPrefabs)
                if (accessory != null)
                    accessory.SetActive(false);

            if (accessoryIndex >= 0 && accessoryIndex < AccessoryPrefabs.Length)
                if (AccessoryPrefabs[accessoryIndex] != null)
                    AccessoryPrefabs[accessoryIndex].SetActive(true);
        }
    }

    private void ApplyFaceCustomization(int eyeStyle, int mouthStyle)
    {
        if (CharacterRenderer != null)
        {
            var props = new MaterialPropertyBlock();
            CharacterRenderer.GetPropertyBlock(props);
            props.SetFloat("_EyeStyle", eyeStyle);
            props.SetFloat("_MouthStyle", mouthStyle);
            CharacterRenderer.SetPropertyBlock(props);
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (CharacterAnimator != null)
            CharacterAnimator.Play(animationName);
    }

    public void SetTalking(bool isTalking)
    {
        if (CharacterAnimator != null)
            CharacterAnimator.SetBool("IsTalking", isTalking);
    }
}
