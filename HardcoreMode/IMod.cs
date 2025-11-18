using BepInEx.Configuration;
using BepInEx.Logging;

namespace HardcoreMode {
    internal interface IMod {
        void Init(ManualLogSource logger, ConfigFile config);
    }
}