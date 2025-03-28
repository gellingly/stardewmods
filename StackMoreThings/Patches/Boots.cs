using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Boots), nameof(Boots.maximumStackSize))]
public static class BootsStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Boots);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class BootsCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Boots && CommonUtils.config.Boots)
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
