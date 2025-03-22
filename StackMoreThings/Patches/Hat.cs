using StardewValley;
using StardewValley.Objects;

public static class StackHatPatches
{
    public static void maximumStackSize_Postfix(Hat __instance, ref int __result)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Hats);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
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
