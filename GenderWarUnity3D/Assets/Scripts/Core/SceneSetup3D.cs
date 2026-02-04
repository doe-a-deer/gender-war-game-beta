using UnityEngine;

namespace GenderWar.Core
{
    /// <summary>
    /// Sets up the 3D restaurant/cafe scene for dates
    /// </summary>
    public class SceneSetup3D : MonoBehaviour
    {
        [Header("Camera")]
        public Camera MainCamera;
        public Transform CameraTarget;
        public float CameraDistance = 5f;
        public float CameraHeight = 2f;
        public float CameraAngle = 15f;

        [Header("Lighting")]
        public Light MainLight;
        public Light FillLight;
        public Light RimLight;
        public Color AmbientColor = new Color(0.1f, 0.07f, 0.09f);

        [Header("Environment")]
        public Transform TablePosition;
        public Transform Chair1Position;
        public Transform Chair2Position;
        public GameObject TablePrefab;
        public GameObject ChairPrefab;

        [Header("Character Positions")]
        public Transform DateCharacterSpot;
        public Transform PlayerCharacterSpot;
        public Transform WaiterSpot;

        [Header("Background")]
        public MeshRenderer BackgroundRenderer;
        public Material RestaurantBackgroundMaterial;

        [Header("Effects")]
        public ParticleSystem AmbientParticles;
        public AudioSource AmbientAudio;

        private void Awake()
        {
            SetupCamera();
            SetupLighting();
            SetupEnvironment();
        }

        private void SetupCamera()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }

            if (MainCamera != null && CameraTarget != null)
            {
                // Position camera for dialogue view
                Vector3 cameraPosition = CameraTarget.position;
                cameraPosition.z -= CameraDistance;
                cameraPosition.y += CameraHeight;
                MainCamera.transform.position = cameraPosition;

                // Angle camera down toward table
                MainCamera.transform.rotation = Quaternion.Euler(CameraAngle, 0, 0);
            }
        }

        private void SetupLighting()
        {
            // Set ambient lighting
            RenderSettings.ambientLight = AmbientColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

            // Configure main light (warm restaurant lighting)
            if (MainLight != null)
            {
                MainLight.type = LightType.Directional;
                MainLight.color = new Color(1f, 0.9f, 0.8f); // Warm white
                MainLight.intensity = 1f;
                MainLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            }

            // Configure fill light
            if (FillLight != null)
            {
                FillLight.type = LightType.Point;
                FillLight.color = new Color(1f, 0.8f, 0.7f); // Warm fill
                FillLight.intensity = 0.5f;
                FillLight.range = 10f;
            }

            // Configure rim light for character separation
            if (RimLight != null)
            {
                RimLight.type = LightType.Directional;
                RimLight.color = new Color(1f, 0.3f, 0.4f); // Pink rim (matches game aesthetic)
                RimLight.intensity = 0.3f;
                RimLight.transform.rotation = Quaternion.Euler(30, 180, 0);
            }
        }

        private void SetupEnvironment()
        {
            // Spawn table
            if (TablePrefab != null && TablePosition != null)
            {
                Instantiate(TablePrefab, TablePosition.position, TablePosition.rotation, transform);
            }

            // Spawn chairs
            if (ChairPrefab != null)
            {
                if (Chair1Position != null)
                {
                    Instantiate(ChairPrefab, Chair1Position.position, Chair1Position.rotation, transform);
                }
                if (Chair2Position != null)
                {
                    Instantiate(ChairPrefab, Chair2Position.position, Chair2Position.rotation, transform);
                }
            }

            // Setup background
            if (BackgroundRenderer != null && RestaurantBackgroundMaterial != null)
            {
                BackgroundRenderer.material = RestaurantBackgroundMaterial;
            }

            // Start ambient effects
            if (AmbientParticles != null)
            {
                AmbientParticles.Play();
            }

            if (AmbientAudio != null)
            {
                AmbientAudio.loop = true;
                AmbientAudio.Play();
            }
        }

        public void SetMoodLighting(string mood)
        {
            switch (mood.ToLower())
            {
                case "tense":
                    MainLight.intensity = 0.7f;
                    MainLight.color = new Color(0.9f, 0.7f, 0.6f);
                    break;

                case "romantic":
                    MainLight.intensity = 0.5f;
                    MainLight.color = new Color(1f, 0.8f, 0.8f);
                    RimLight.intensity = 0.5f;
                    break;

                case "chaotic":
                    MainLight.intensity = 1.2f;
                    MainLight.color = new Color(1f, 1f, 1f);
                    break;

                case "ending":
                    MainLight.intensity = 0.3f;
                    RenderSettings.ambientLight = new Color(0.05f, 0.03f, 0.04f);
                    break;

                default: // normal
                    MainLight.intensity = 1f;
                    MainLight.color = new Color(1f, 0.9f, 0.8f);
                    break;
            }
        }

        public void TransitionToEnding()
        {
            SetMoodLighting("ending");
        }
    }
}
