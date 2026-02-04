using UnityEngine;
using System;
using System.Collections.Generic;
using GenderWar.Core;

namespace GenderWar.Characters
{
    /// <summary>
    /// Manages character sprites, expressions, and 3D character display
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [Header("Character Display")]
        public Transform DateCharacterPosition;
        public Transform PlayerCharacterPosition;
        public Transform WaiterCharacterPosition;

        [Header("Character Data")]
        public CharacterData IncelCharacter;
        public CharacterData FemcelCharacter;
        public CharacterData PerformativeCharacter;
        public CharacterData BopCharacter;
        public CharacterData WaiterCharacter;
        public CharacterData PlayerCharacter;

        [Header("3D Character Prefabs")]
        public GameObject Character3DPrefab;

        private GameObject currentDateCharacter;
        private GameObject currentPlayerCharacter;
        private CharacterData currentDateData;

        // Events
        public event Action<string, string> OnExpressionChanged;

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

        public void SetupCharactersForRoute(RouteType route)
        {
            currentDateData = GetCharacterData(route);
            SpawnCharacters();
        }

        private void SpawnCharacters()
        {
            // Clear existing characters
            if (currentDateCharacter != null) Destroy(currentDateCharacter);
            if (currentPlayerCharacter != null) Destroy(currentPlayerCharacter);

            // Spawn date character
            if (Character3DPrefab != null && DateCharacterPosition != null)
            {
                currentDateCharacter = Instantiate(Character3DPrefab, DateCharacterPosition);
                var display = currentDateCharacter.GetComponent<Character3DDisplay>();
                if (display != null)
                {
                    display.Initialize(currentDateData);
                }
            }

            // Spawn player character
            if (Character3DPrefab != null && PlayerCharacterPosition != null)
            {
                currentPlayerCharacter = Instantiate(Character3DPrefab, PlayerCharacterPosition);
                var display = currentPlayerCharacter.GetComponent<Character3DDisplay>();
                if (display != null)
                {
                    display.Initialize(PlayerCharacter);
                    // Apply player customization
                    var appearance = GameManager.Instance.CurrentState.Appearance;
                    display.ApplyPlayerAppearance(appearance);
                }
            }
        }

        public void SetDateExpression(string expression)
        {
            if (currentDateCharacter != null)
            {
                var display = currentDateCharacter.GetComponent<Character3DDisplay>();
                display?.SetExpression(expression);
            }
            OnExpressionChanged?.Invoke("date", expression);
        }

        public void SetPlayerExpression(string expression)
        {
            if (currentPlayerCharacter != null)
            {
                var display = currentPlayerCharacter.GetComponent<Character3DDisplay>();
                display?.SetExpression(expression);
            }
            OnExpressionChanged?.Invoke("player", expression);
        }

        public CharacterData GetCharacterData(RouteType route)
        {
            return route switch
            {
                RouteType.Incel => IncelCharacter,
                RouteType.Femcel => FemcelCharacter,
                RouteType.Performative => PerformativeCharacter,
                RouteType.Bop => BopCharacter,
                _ => null
            };
        }

        public CharacterData GetCurrentDateCharacter() => currentDateData;
    }
}
