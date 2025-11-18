using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HardcoreMode {
    public class MainMenuMod: IMod {
        private ConfigEntry<bool> _configCameraSway;

        public void Init(ManualLogSource logger, ConfigFile config)
        {
            _configCameraSway = config.Bind("MainMenu.Misc",
                "Remove Camera sway",
                false,
                "Enabling this stops the main menu camera from moving");

            SceneManager.sceneLoaded += (scene, mode) => {
                if (scene.name == "Menu") {
                    OnMenuLoaded();
                }
            };

            SceneManager.sceneUnloaded += (scene) => {
                if (scene.name == "Menu") {
                    OnMenuUnloaded();
                }
            };
        }

        private void OnMenuLoaded() {
            Debug.Log("On menu loaded");
            ToggleCamMovement(_configCameraSway.Value);
            _configCameraSway.SettingChanged += OnConfigCameraSwayChanged;

            AddDifficultyButtons();
        }
        
        
        private void OnMenuUnloaded() {
            _configCameraSway.SettingChanged -= OnConfigCameraSwayChanged;
        }

        private void OnConfigCameraSwayChanged(object obj, EventArgs args) {
            Debug.LogError("Setting changed!!!");
            ToggleCamMovement(_configCameraSway.Value);
        }


        private static void ToggleCamMovement(bool isDisabled) {
            var isActive = !isDisabled;
            var menuCam = GameObject.FindAnyObjectByType<MenuCamera>();
            Debug.Log("Menu cam found: " + menuCam + " " + GameObject.FindObjectsByType<MenuCamera>(FindObjectsSortMode.InstanceID).Length);
            if (menuCam) {
                menuCam.enabled = isActive;
                menuCam.transform.Find("Bird").gameObject.SetActive(isActive);
            }
        }

        private static void AddDifficultyButtons() {
            var c = GameObject.Find("Menu Canvas");
            var hard = c.transform.Find("Difficulty/DiffcultySelection/MapSelection/Hard").gameObject;
            var normal = c.transform.Find("Difficulty/DiffcultySelection/MapSelection/Normal").gameObject;
            var relaxed = c.transform.Find("Difficulty/DiffcultySelection/MapSelection/Relaxed").gameObject;


            var expert = GameObject.Instantiate(relaxed, hard.transform.parent);
            var expertTexts = expert.GetComponentsInChildren<TMP_Text>();
            expertTexts[0].text = "Expert";
            expertTexts[1].text = "Extra challenge. No partial deliveries allowed";


            var hardcore = GameObject.Instantiate(relaxed, expert.transform.parent);
            var hardcoreTexts = hardcore.GetComponentsInChildren<TMP_Text>();
            hardcoreTexts[0].text = "Hardcore";
            hardcoreTexts[1].text = "For experienced pilots only - expert rules, and deaths from explosions are permanent.";


            var diffSelector = c.transform.Find("Difficulty/DiffcultySelection").gameObject;
            var oldDiffSelector = diffSelector.GetComponent<DifficultySelector>();
        
            var newDiffSelector = diffSelector.AddComponent<NewDifficultySelector>();
            newDiffSelector.selectedColor = oldDiffSelector.selectedColor;
            newDiffSelector.deselectedColor = oldDiffSelector.deselectedColor;
            newDiffSelector.Buttons = new Dictionary<GameObject, MyDifficulty> {
                { normal, MyDifficulty.Normal },
                { relaxed, MyDifficulty.Relaxed },
                { hard, MyDifficulty.Hard },
                { expert, MyDifficulty.Expert },
                { hardcore, MyDifficulty.Hardcore },

            }.ToDictionary(kv => kv.Key.GetComponent<Button>(), kv => kv.Value);
            GameObject.Destroy(oldDiffSelector);

            var bg = c.transform.Find("Difficulty/DiffcultySelection/Background").gameObject;
            var sel = c.transform.Find("Difficulty/DiffcultySelection/MapSelection").gameObject;


            // Add ContentSizeFitter to Vertical Layout Group
            var fitter = sel.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            sel.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(10, 10, 10, 10);

            // Source GameObject with Image component
            GameObject sourceGO = bg;
            GameObject targetGO = sel;
            
            // Get the Image component from source
            Image sourceImage = sourceGO.GetComponent<UnityEngine.UI.Image>();
            Image newImage = MainMenuMod.CopyComponent(sourceImage, targetGO);
            newImage.sprite = sourceImage.sprite;
            newImage.type = Image.Type.Sliced;
            newImage.pixelsPerUnitMultiplier = 4;
            sourceGO.SetActive(false);

            var back = c.transform.Find("Difficulty/DiffcultySelection/Back").gameObject;
            var start = c.transform.Find("Difficulty/DiffcultySelection/StartGame").gameObject;

            MainMenuMod.CreateHorizontalContainer(sel, new List<GameObject> { back, start });
        }

        private static GameObject CreateHorizontalContainer(GameObject target, List<GameObject> sources) {
            // Create container under target
            GameObject container = new GameObject("container");
            container.transform.SetParent(target.transform, false);

            // Add Horizontal Layout Group
            HorizontalLayoutGroup layoutGroup = container.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();


            // Optional: Configure layout group settings
            layoutGroup.childControlWidth = false;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.spacing = 20f;

            var rect = container.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 50);
            // Reparent all source objects to container
            foreach (GameObject source in sources) {
                source.transform.SetParent(container.transform, false);
            }

            return container;
        }
        private static T CopyComponent<T>(T original, GameObject destination) where T : Component {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);

            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (var field in fields) {
                field.SetValue(copy, field.GetValue(original));
            }

            return copy as T;
        }

    }
}