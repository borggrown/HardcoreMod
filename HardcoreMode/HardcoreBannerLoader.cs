using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace HardcoreMode {
    internal class HardcoreBannerLoader {
        private readonly ManualLogSource _logger;
        
        public HardcoreBannerLoader(ManualLogSource logger) {
            this._logger = logger;
        }

        public UIDocument LoadUIFromBundle(string bundlePath) {
            var bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null) {
                _logger.LogError("Failed to load bundle!");
                return null;
            }

            _logger.LogInfo("Bundle loaded!");

            // Load the UXML
            var visualTree = bundle.LoadAsset<VisualTreeAsset>("HardcoreBanner");

            // Load PanelSettings
            var panelSettings = bundle.LoadAsset<PanelSettings>("PanelSettings");

            // Optional: Load theme
            var theme = bundle.LoadAsset<ThemeStyleSheet>("UnityDefaultRuntimeTheme");

            // Create UI
            var go = new GameObject("RuntimeUI");
            var doc = go.AddComponent<UIDocument>();

            // Use the PanelSettings from bundle (or create new one)
            doc.panelSettings = panelSettings;


            // Clone the UXML tree
            var root = doc.rootVisualElement;
            visualTree.CloneTree(root);


            _logger.LogInfo("UI created successfully!");

            doc.rootVisualElement.style.display = DisplayStyle.None;
            return doc;
        }

    }
}
