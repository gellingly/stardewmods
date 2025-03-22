using StardewValley;
using StardewValley.Objects;

public class StackRingsPatches
{
    public static void maximumStackSize_Postfix(ref int __result, Ring __instance)
    {
        CommonUtils.setMaxStackSize(ref __result, CommonUtils.config.Rings);
    }

    public static void canStackWith_Postfix(ISalable other, ref bool __result, Item __instance)
    {
        try
        {
            if (__instance is Ring && CommonUtils.config.Rings)
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
