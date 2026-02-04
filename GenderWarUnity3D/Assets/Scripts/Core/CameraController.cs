using UnityEngine;
using System.Collections;

namespace GenderWar.Core
{
    /// <summary>
    /// Handles camera movements, focus changes, and cinematic shots
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Camera Settings")]
        public Camera MainCamera;
        public float DefaultFOV = 60f;
        public float CloseupFOV = 45f;
        public float TransitionSpeed = 2f;

        [Header("Focus Targets")]
        public Transform TableCenter;
        public Transform DateCharacterFocus;
        public Transform PlayerCharacterFocus;
        public Transform TwoShotPosition;

        [Header("Camera Positions")]
        public Transform DefaultPosition;
        public Transform DateCloseupPosition;
        public Transform PlayerCloseupPosition;
        public Transform WidePosition;

        [Header("Shake Settings")]
        public float ShakeIntensity = 0.1f;
        public float ShakeDuration = 0.3f;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Coroutine currentTransition;

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

            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }

            if (MainCamera != null)
            {
                originalPosition = MainCamera.transform.position;
                originalRotation = MainCamera.transform.rotation;
            }
        }

        public void FocusOnDate()
        {
            if (DateCloseupPosition != null)
            {
                TransitionTo(DateCloseupPosition.position, DateCloseupPosition.rotation, CloseupFOV);
            }
        }

        public void FocusOnPlayer()
        {
            if (PlayerCloseupPosition != null)
            {
                TransitionTo(PlayerCloseupPosition.position, PlayerCloseupPosition.rotation, CloseupFOV);
            }
        }

        public void FocusOnBoth()
        {
            if (TwoShotPosition != null)
            {
                TransitionTo(TwoShotPosition.position, TwoShotPosition.rotation, DefaultFOV);
            }
        }

        public void ReturnToDefault()
        {
            if (DefaultPosition != null)
            {
                TransitionTo(DefaultPosition.position, DefaultPosition.rotation, DefaultFOV);
            }
            else
            {
                TransitionTo(originalPosition, originalRotation, DefaultFOV);
            }
        }

        public void GoWide()
        {
            if (WidePosition != null)
            {
                TransitionTo(WidePosition.position, WidePosition.rotation, DefaultFOV + 10);
            }
        }

        private void TransitionTo(Vector3 targetPosition, Quaternion targetRotation, float targetFOV)
        {
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
            }
            currentTransition = StartCoroutine(TransitionCoroutine(targetPosition, targetRotation, targetFOV));
        }

        private IEnumerator TransitionCoroutine(Vector3 targetPosition, Quaternion targetRotation, float targetFOV)
        {
            float elapsed = 0f;
            float duration = 1f / TransitionSpeed;

            Vector3 startPosition = MainCamera.transform.position;
            Quaternion startRotation = MainCamera.transform.rotation;
            float startFOV = MainCamera.fieldOfView;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);

                MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                MainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                MainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

                yield return null;
            }

            MainCamera.transform.position = targetPosition;
            MainCamera.transform.rotation = targetRotation;
            MainCamera.fieldOfView = targetFOV;
        }

        public void Shake(float intensity = -1, float duration = -1)
        {
            if (intensity < 0) intensity = ShakeIntensity;
            if (duration < 0) duration = ShakeDuration;

            StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            Vector3 startPosition = MainCamera.transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float dampening = 1f - (elapsed / duration);

                float x = Random.Range(-1f, 1f) * intensity * dampening;
                float y = Random.Range(-1f, 1f) * intensity * dampening;

                MainCamera.transform.position = startPosition + new Vector3(x, y, 0);

                yield return null;
            }

            MainCamera.transform.position = startPosition;
        }

        public void PunchZoom(float amount = 5f, float duration = 0.2f)
        {
            StartCoroutine(PunchZoomCoroutine(amount, duration));
        }

        private IEnumerator PunchZoomCoroutine(float amount, float duration)
        {
            float startFOV = MainCamera.fieldOfView;
            float targetFOV = startFOV - amount;
            float elapsed = 0f;

            // Zoom in
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                MainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
                yield return null;
            }

            // Zoom out
            elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                MainCamera.fieldOfView = Mathf.Lerp(targetFOV, startFOV, t);
                yield return null;
            }

            MainCamera.fieldOfView = startFOV;
        }

        // Called when dramatic moments happen in dialogue
        public void OnDramaticMoment(string momentType)
        {
            switch (momentType.ToLower())
            {
                case "reveal":
                    PunchZoom(8f, 0.3f);
                    break;
                case "angry":
                    Shake(0.15f, 0.4f);
                    break;
                case "surprise":
                    PunchZoom(5f, 0.2f);
                    break;
                case "ending":
                    GoWide();
                    break;
            }
        }
    }
}
