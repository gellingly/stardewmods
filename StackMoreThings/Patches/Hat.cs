using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Hat), nameof(Hat.maximumStackSize))]
public static class HatMaxStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Hats);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class HatCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Hat && CommonUtils.config.Hats)
            {
                __result = CommonUtils.commonCompares(__instance, other);
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
