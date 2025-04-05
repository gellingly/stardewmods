using System.Reflection.Emit;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;

namespace LessSecretLogoScreenSecret;

internal sealed class ModEntry : Mod
{
    public static ModConfig Config;

    public override void Entry(IModHelper helper)
    {
        Config = this.Helper.ReadConfig<ModConfig>();
        var harmony = new Harmony(this.ModManifest.UniqueID);
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(TitleMenu), nameof(TitleMenu.receiveLeftClick))]
public class LogoScreenSecret
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        // Patch the rand < probability check to use the config value instead of 0.02
        var codes = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codes.Count; i++)
        {
            if (
                codes[i].opcode == OpCodes.Ldc_R8
                && (double)codes[i].operand == 0.02
                && 0 <= i - 1
                && codes[i - 1].opcode == OpCodes.Callvirt
                && codes[i - 1].operand
                    == AccessTools.Method(typeof(Random), nameof(Random.NextDouble)) as object
                && i + 1 < codes.Count
                && codes[i + 1].opcode == OpCodes.Bge_Un_S
            )
            {
                codes.RemoveAt(i);
                codes.Insert(i, new CodeInstruction(OpCodes.Ldc_R8, ModEntry.Config.Probability));
                break;
            }
        }

        return codes.AsEnumerable();
    }
}
