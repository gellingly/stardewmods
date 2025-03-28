using HarmonyLib;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.maximumStackSize))]
public class MaxStackSizeGeneral
{
    public static void Postfix(ref int __result)
    {
        try
        {
            if (__result == 999)
            {
                __result = CommonUtils.config.MaxStackSize;
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
