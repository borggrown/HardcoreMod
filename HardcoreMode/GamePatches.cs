using BepInEx;
using BepInEx.Harmony;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

#pragma warning disable IDE0051 // Naming Styles
namespace HardcoreMode {
    internal class GamePatches {

        
        public static ConfigEntry<MyDifficulty> OnlyWholeDeliveriesMinDifficulty;
        public static ConfigEntry<MyDifficulty> PreventResetsInMotionMinDifficulty;

        public static void Init(ConfigFile config) {
            OnlyWholeDeliveriesMinDifficulty = config.Bind("GameRules",
                "OnlyWholeDeliveriesMinDifficulty", 
                MyDifficulty.Expert,
                "The minimum difficulty required to stop accepting partial deliveries in the game.");
            
            PreventResetsInMotionMinDifficulty = config.Bind("GameRules",
                "PreventResetsInMotionMinDifficulty", 
                MyDifficulty.Expert,
                "The minimum difficulty required to prevent resets while mid-air.");

            Harmony.CreateAndPatchAll(typeof(GamePatches));
        }
        
        /*
        [HarmonyPatch(typeof(LoadPanel), nameof(LoadPanel.Load))] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        static bool PatchLoadPanelLoadHardcoreFiles() {
            return true;
        }

        [HarmonyPatch(typeof(LoadButton), nameof(LoadButton.Init))] // Specify target method with HarmonyPatch attribute
        [HarmonyPostfix]
        static void PatchLoadButtonInit(LoadButton __instance) {
            __instance.playtimeText.text += " (Hardcore)";
            __instance.playtimeText.color = UnityEngine.Color.red;
        }

        [HarmonyPatch(typeof(LoadButton), nameof(LoadButton.Press))] // Specify target method with HarmonyPatch attribute
        [HarmonyPrefix]
        static bool PatchLoadButtonPress() {
            return false;
        }
        */

        /**
         * Mark player as dead on plane explosion in hardcore mode
         */
        [HarmonyPatch(typeof(PlaneController), nameof(PlaneController.ExplodePlane))] 
        [HarmonyPostfix]
        static void AfterExplodePlane() {
            var gameData = Singleton<GameModeData>.Instance;
            if ((MyDifficulty)gameData.currentDifficulty == MyDifficulty.Hardcore) {
                gameData.currentDifficulty = (Difficulty)MyDifficulty.Dead;
                Singleton<PersistentStorage>.Instance.autoSave.CreateSave();
            }
        }

        /**
         * Modify GameOver screen in case of death in hardcore mode
         */
        [HarmonyPatch(typeof(GameOver), "Update")] 
        [HarmonyPrefix]
        static bool BeforeGameOverUpdate(GameOver __instance) {
            var gameData = Singleton<GameModeData>.Instance;
            if ((MyDifficulty)gameData.currentDifficulty == MyDifficulty.Dead) {
                __instance.text.text = "You died";
                __instance.textShadow.text = __instance.text.text;
                __instance.content.GetComponentsInChildren<Button>().ToList().ForEach(b => b.gameObject.SetActive(false));

                __instance.content.transform.Find("Header (1)").GetComponent<TMPro.TMP_Text>().text = "Quit the current game to start a new one";

                var plane = GameObject.FindAnyObjectByType<PlaneController>();
                if (plane != null) {
                    plane.gameObject.SetActive(false);
                }
                ReverseStartReload(__instance);
                return false;
            }
            return true;
        }

        /**
         * Necessary to start animation on death
         */
        [HarmonyPatch(typeof(GameOver), "StartReload")] 
        [HarmonyReversePatch]
        static void ReverseStartReload(GameOver __instance) {
            throw new NotImplementedException("It's a stub");
        }

        /**
         * Prevent partial deliveries in expert and hardcore modes
         */
        [HarmonyPatch(typeof(DeliveryQuest), "CheckCompletion")] 
        [HarmonyPrefix]
        static bool BeforeDeliveryQuestCheckCompletion(DeliveryQuest __instance) {
            if ((MyDifficulty)Singleton<GameModeData>.Instance.currentDifficulty >= OnlyWholeDeliveriesMinDifficulty.Value 
                && Singleton<CargoInventory>.Instance.GetCargoCount(__instance.cargoType) < __instance.amount) {
                return false;
            }
            return true;
        }

        // Patch for MenuDialog.Update to disable "Save As" button in hardcore mode
        [HarmonyPatch(typeof(MenuDialog), "Update")]
        [HarmonyPostfix]
        static void AfterMenuDialogUpdate(MenuDialog __instance) {
            var gameData = Singleton<GameModeData>.Instance;
            if ((MyDifficulty)gameData.currentDifficulty == MyDifficulty.Hardcore) {
                __instance.saveAsButton.interactable = false;
                __instance.saveAsButtonHider.SetActive(true);
                __instance.saveAsButton.GetComponent<TooltipTrigger>().enabled = true;
                __instance.saveAsButton.GetComponent<TooltipTrigger>().settings.prefab.GetComponentInChildren<TMP_Text>().text = "Multiple save files are disabled in hardcore.";
            }
        }

        [HarmonyPatch(typeof(StartStopButton), nameof(StartStopButton.ReloadFlyMode))]
        [HarmonyPrefix]
        static bool BeforeReloadFlyMode() {
            return !ShouldPreventResets();
        }
        
        [HarmonyPatch(typeof(StartStopButton), nameof(StartStopButton.ResetAtLastAirport))]
        [HarmonyPrefix]
        static bool BeforeResetAtLastAirport() {
            return !ShouldPreventResets();
        }
        
        [HarmonyPatch(typeof(StartStopButton), nameof(StartStopButton.OpenWarning))]
        [HarmonyPrefix]
        static bool BeforeOpenWarning() {
            return !ShouldPreventResets();
        }
        
        [HarmonyPatch(typeof(StartStopButton), nameof(StartStopButton.ResetAtLastAirport))]
        [HarmonyPostfix]
        static void AfterResetAtLastAirport(StartStopButton __instance) {
            if (ShouldPreventResets()) {
                __instance.warning.SetActive(false);
            }
        }
        
        private static bool ShouldPreventResets() {
            return (MyDifficulty)Singleton<GameModeData>.Instance.currentDifficulty >= PreventResetsInMotionMinDifficulty.Value 
                && Singleton<PlaneContainer>.Instance.GetVelocityMagintude() > 25f;
        }

#pragma warning restore IDE0051 // Naming Styles
    }
}
