using UnityEngine;
using GenderWar.Core;

namespace GenderWar.Characters
{
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

        // Color presets for player customization
        private readonly Color[] SkinTones = new Color[]
        {
            new Color(0.7f, 0.9f, 0.7f), // Light green
            new Color(0.9f, 0.8f, 0.7f), // Tan
            new Color(0.6f, 0.4f, 0.3f), // Brown
            new Color(1f, 0.7f, 0.8f),   // Pink
            new Color(0.7f, 0.8f, 1f)    // Light blue
        };

        public void Initialize(CharacterData data)
        {
            characterData = data;
            if (data == null) return;

            // Setup 3D mesh
            if (CharacterMesh != null && data.Character3DMesh != null)
            {
                CharacterMesh.mesh = data.Character3DMesh;
            }

            // Setup material
            if (CharacterRenderer != null && data.Character3DMaterial != null)
            {
                CharacterRenderer.material = data.Character3DMaterial;
            }

            // Setup expression sprite
            SetExpression("neutral");
        }

        public void SetExpression(string expression)
        {
            currentExpression = expression;

            if (characterData == null) return;

            // Update 2D expression overlay
            if (ExpressionSprite != null)
            {
                ExpressionSprite.sprite = characterData.GetExpression(expression);
            }

            // Update 3D material if available
            if (CharacterRenderer != null)
            {
                var mat = characterData.GetExpressionMaterial(expression);
                if (mat != null)
                {
                    CharacterRenderer.material = mat;
                }
            }

            // Trigger animation if available
            if (CharacterAnimator != null)
            {
                CharacterAnimator.SetTrigger(expression);
            }
        }

        public void ApplyPlayerAppearance(PlayerAppearance appearance)
        {
            // Apply skin tone
            if (SkinRenderer != null && appearance.SkinTone < SkinTones.Length)
            {
                var mat = SkinRenderer.material;
                mat.color = SkinTones[appearance.SkinTone];
            }

            // Apply accessory
            ApplyAccessory(appearance.Accessory);

            // Apply face customization through shader or texture
            ApplyFaceCustomization(appearance.EyeStyle, appearance.MouthStyle);
        }

        private void ApplyAccessory(int accessoryIndex)
        {
            // Disable all accessories first
            if (AccessoryPrefabs != null)
            {
                foreach (var accessory in AccessoryPrefabs)
                {
                    if (accessory != null)
                        accessory.SetActive(false);
                }

                // Enable selected accessory
                if (accessoryIndex >= 0 && accessoryIndex < AccessoryPrefabs.Length)
                {
                    if (AccessoryPrefabs[accessoryIndex] != null)
                        AccessoryPrefabs[accessoryIndex].SetActive(true);
                }
            }
        }

        private void ApplyFaceCustomization(int eyeStyle, int mouthStyle)
        {
            // This would typically update a shader or texture atlas
            // For now, we'll use material property blocks
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
            {
                CharacterAnimator.Play(animationName);
            }
        }

        public void SetTalking(bool isTalking)
        {
            if (CharacterAnimator != null)
            {
                CharacterAnimator.SetBool("IsTalking", isTalking);
            }
        }
    }
}
