using HarmonyLib;
using StardewValley;
using StardewValley.Objects.Trinkets;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Trinket), nameof(Trinket.maximumStackSize))]
public static class TrinketStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Trinkets);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class TrinketCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Trinket && CommonUtils.config.Trinkets)
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
