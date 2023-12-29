using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using GameNetcodeStuff;

namespace NoMorePush
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class NoMorePush : BaseUnityPlugin
    {
        
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        internal static ManualLogSource MyLogger;
        
        private void Awake()
        {
            MyLogger = BepInEx.Logging.Logger.CreateLogSource("NoMorePush");
            
            MyLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerControllerB))]
        [HarmonyPatch("Update")]
        public static class PlayerControllerBUpdatePatch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                
                // This is dumb. Seriously. I hope the update won't change the magic number. Or add another magic number with 12F on update loop.
                foreach (var code in codes.Where(code => code.opcode == OpCodes.Ldc_R4 && (int)((float)code.operand * 100.0f) == 120))
                {
                    code.operand = 0.0f;
                    break;
                }
                return codes.AsEnumerable();
            }
        }
    }
}
