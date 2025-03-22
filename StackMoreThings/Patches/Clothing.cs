using StardewValley;
using StardewValley.Objects;

public static class StackClothingPatches
{
    public static void maximumStackSize_Postfix(Clothing __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Clothing);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
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
