using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HardcoreMode
{
    [BepInPlugin("com.borggrown.plugins.hardcoremode", "Hardcore Mode", "0.1.0")]
    public class Plugin: BaseUnityPlugin {

        public void Awake()
        {
            Logger.LogInfo("Hardcore Mode Plugin Loaded");
            GamePatches.Init(Config);
            
            Harmony.CreateAndPatchAll(typeof(GamePatches));
            Logger.LogInfo("Patches Applied");
            
            new HardcoreBannerMod().Init(Logger, Config);
            new MainMenuMod().Init(Logger, Config);
        }
    }
}
