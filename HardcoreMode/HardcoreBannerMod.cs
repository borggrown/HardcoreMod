using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HardcoreMode {
    internal class HardcoreBannerMod: IMod {
        private ManualLogSource _logger;
        
        private ConfigEntry<bool> _configShowHardcoreBanner;
        private UIDocument _hardcoreBanner;

        public void Init(ManualLogSource logger, ConfigFile config) {
            _logger = logger;
            logger.LogInfo("HUH");
            logger.LogInfo("Hardcore banner loaded: " + _hardcoreBanner);

            _configShowHardcoreBanner = config.Bind("Hardcore",
                "DisplayBanner",
                true,
                "Whether or not to show the red banner while playing hardcore saves");

            _configShowHardcoreBanner.SettingChanged += (s, e) => {
                if (!_configShowHardcoreBanner.Value) {
                    Hide();
                } else {
                    TryShow();
                }
            };

            SceneManager.sceneLoaded += (scene, mode) => {
                if (scene.name == "Fader") {
                    _hardcoreBanner = LoadBannerObject();
                } else if (scene.name == "Game") {
                    TryShow();
                }
            };

            SceneManager.sceneUnloaded += scene => {
                if (scene.name == "Game") {
                    Hide();
                }
            };
        }
        
        private void TryShow() {
            var gameData = Singleton<GameModeData>.Instance;
            _logger.LogInfo($"Current difficulty: {gameData.currentDifficulty}");
            _logger.LogInfo("Banner config: " + _configShowHardcoreBanner);

            _logger.LogInfo("Banner: " + _hardcoreBanner);

            if ((MyDifficulty)gameData.currentDifficulty == MyDifficulty.Hardcore && _configShowHardcoreBanner.Value) {
                _hardcoreBanner.rootVisualElement.style.display = DisplayStyle.Flex;
            }
        }

        private void Hide() {
            _hardcoreBanner.rootVisualElement.style.display = DisplayStyle.None;
        }

        private UIDocument LoadBannerObject() {
            string pluginFolder = Paths.PluginPath;
            string bundlePath = Path.Combine(pluginFolder, "HardcoreMode", "assets", "uibundle");
            return new HardcoreBannerLoader(_logger).LoadUIFromBundle(bundlePath);
        }
    }
}