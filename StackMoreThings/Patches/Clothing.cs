using HarmonyLib;
using StardewValley;
using StardewValley.Objects;

namespace StackMoreThings.Patches;

[HarmonyPatch(typeof(Clothing), nameof(Clothing.maximumStackSize))]
public static class ClothingStackSize
{
    public static void Postfix(ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Clothing);
    }
}

[HarmonyPatch(typeof(Item), nameof(Item.canStackWith))]
public static class ClothingCanStackWith
{
    public static void Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Clothing a && CommonUtils.config.Clothing)
            {
                __result =
                    other is Clothing b
                    && CommonUtils.commonCompares(__instance, other)
                    && (!a.dyeable.Value || a.clothesColor.Equals(b.clothesColor));
            }
        }
        catch (Exception ex)
        {
            CommonUtils.harmonyExceptionPrint(ex);
        }
    }
}
